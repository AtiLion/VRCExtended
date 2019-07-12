using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRC;

using VRChat;

using VRCTools;

using UnityEngine;

using VRC_AvatarDescriptor = VRCSDK2.VRC_AvatarDescriptor;

namespace VRCExtended
{
    internal class ExtendedUser : VRCEPlayer
    {
        #region Individual Volume Variables
        private float _volumeAvatar = 1f;
        private float _volumeVoice = 1f;
        #endregion

        #region AntiCrash Variables
        private static Shader _defaultShader;

        private Dictionary<Material, Shader> _fallbackShaders = new Dictionary<Material, Shader>();
        private Dictionary<ParticleSystem, int> _fallbackParticleLimits = new Dictionary<ParticleSystem, int>();
        private Dictionary<MeshRenderer, MeshFilter> _fallbackMeshes = new Dictionary<MeshRenderer, MeshFilter>();
        #endregion

        #region Local Colliders Properties
        public List<DynamicBoneCollider> BoneColliders { get; private set; } = new List<DynamicBoneCollider>();
        public List<DynamicBone> Bones { get; private set; } = new List<DynamicBone>();
        #endregion

        #region Individual Volume Properties
        public float VolumeAvatar
        {
            get => _volumeAvatar;
            set
            {
                float prc = value / 100;

                foreach(AudioSource audio in Sounds)
                    audio.volume -= prc * audio.volume;
            }
        }
        public float VolumeVoice
        {
            get => _volumeVoice;
            set
            {
                foreach(AudioSource audio in Avatar.GetComponentsInChildren<AudioSource>())
                {
                    audio.volume = value;
                    _volumeVoice = value;
                }
                /*if (Voice == null)
                    return;
                ExtendedLogger.Log("Test");
                Voice.volume = value;
                _volumeVoice = value;*/
            }
        }
        #endregion

        public ExtendedUser(VRCEPlayer player) : base(player.Player) => Setup();
        public ExtendedUser(Player player) : base(player) => Setup();
        public ExtendedUser() : base() => Setup();

        #region Setup Functions
        private void Setup()
        {
            AvatarManager.OnAvatarCreated += OnAvatarCreated;

            ExtendedUser self = ExtendedServer.Users.FirstOrDefault(a => a.APIUser == APIUser);
            if(self != null)
            {
                _volumeAvatar = self._volumeAvatar;
                _volumeVoice = self._volumeVoice;

                BoneColliders = self.BoneColliders;
                Bones = self.Bones;

                _fallbackShaders = self._fallbackShaders;
                _fallbackParticleLimits = self._fallbackParticleLimits;
                _fallbackMeshes = self._fallbackMeshes;
            }
        }
        #endregion

        #region Avatar Events
        void OnAvatarCreated(GameObject avatar, VRC_AvatarDescriptor avatarDescriptor, bool _1)
        {
            if (ModPrefs.GetBool("vrcextended", "userSpecificVolume"))
                VolumeAvatar = _volumeAvatar;
            if(ModPrefs.GetBool("vrcextended", "localColliders"))
            {
                foreach(ExtendedUser user in ExtendedServer.Users)
                {
                    if (user.APIUser == APIUser)
                        continue;

                    if (ModPrefs.GetBool("vrcextended", "multiLocalColliders") || (ModPrefs.GetBool("vrcextended", "selfLocalColliders") && IsSelf) || user.APIUser == Instance.APIUser)
                        foreach (DynamicBone bone in user.Bones)
                            foreach (DynamicBoneCollider collider in BoneColliders)
                                if (bone.m_Colliders.Contains(collider))
                                    bone.m_Colliders.Remove(collider);
                }
                BoneColliders.Clear();
                Bones.Clear();

                BoneColliders.AddRange(Avatar.GetComponentsInChildren<DynamicBoneCollider>(true));
                Bones.AddRange(Avatar.GetComponentsInChildren<DynamicBone>(true).Where(a => a.m_Colliders.Count > 1));

                foreach(ExtendedUser user in ExtendedServer.Users)
                {
                    if (user.APIUser == APIUser)
                        continue;

                    if (ModPrefs.GetBool("vrcextended", "multiLocalColliders") || (ModPrefs.GetBool("vrcextended", "selfLocalColliders") && IsSelf) || user.APIUser == Instance.APIUser)
                        foreach (DynamicBone bone in Bones)
                            foreach (DynamicBoneCollider collider in user.BoneColliders)
                                bone.m_Colliders.Add(collider);
                    if(ModPrefs.GetBool("vrcextended", "multiLocalColliders") || (ModPrefs.GetBool("vrcextended", "selfLocalColliders") && user.APIUser == Instance.APIUser) || IsSelf)
                        foreach (DynamicBone bone in user.Bones)
                            foreach (DynamicBoneCollider collider in BoneColliders)
                                bone.m_Colliders.Add(collider);
                }
                ExtendedLogger.Log("Added local colliders to " + APIUser.displayName + "!");
            }
            if(ModPrefs.GetBool("vrcextended", "antiCrasher"))
            {
                _fallbackShaders.Clear();
                RemoveCrashShaders();

                _fallbackParticleLimits.Clear();
                LimitParticles();

                _fallbackMeshes.Clear();
                RemoveCrashMesh();
            }
        }
        #endregion

        #region AntiCrash Functions
        public void RemoveCrashShaders()
        {
            if (!ModPrefs.GetBool("vrcextended", "antiCrasher"))
                return;
            if (_defaultShader == null)
                _defaultShader = Shader.Find("Standard");

            // Change cached
            if (_fallbackShaders.Count > 0)
            {
                foreach(Material material in _fallbackShaders.Keys)
                {
                    material.shader = _defaultShader;
                    ExtendedLogger.LogWarning("Removed blacklisted shader " + material.shader.name + " from " + APIUser.displayName);
                }
                return;
            }

            // Change non-cached
            Renderer[] renderers = Avatar.GetComponentsInChildren<Renderer>(true);
            int rLength = renderers.Length;

            for(int i = 0; i < rLength; i++)
            {
                foreach(Material material in renderers[i].materials)
                {
                    foreach(string blacklistedName in AntiCrasherConfig.Instance.BlacklistedShaders)
                    {
                        if(material.shader.name.ToLower().Contains(blacklistedName))
                        {
                            if (!_fallbackShaders.ContainsKey(material))
                                _fallbackShaders.Add(material, material.shader);
                            material.shader = _defaultShader;

                            ExtendedLogger.LogWarning("Removed blacklisted shader " + material.shader.name + " from " + APIUser.displayName);
                            break;
                        }
                    }
                }
            }
        }
        public void RestoreCrashShaders()
        {
            if (ModPrefs.GetBool("vrcextended", "antiCrasher"))
                return;
            if (_fallbackShaders.Count < 1)
                return;

            foreach(Material material in _fallbackShaders.Keys)
            {
                material.shader = _fallbackShaders[material];
                ExtendedLogger.LogWarning("Restored blacklisted shader " + material.shader.name + " to " + APIUser.displayName);
            }
        }

        public void LimitParticles()
        {
            if (!ModPrefs.GetBool("vrcextended", "antiCrasher"))
                return;
            int maxPerSystem;

            // Change cached
            if (_fallbackParticleLimits.Count > 0)
            {
                maxPerSystem = AntiCrasherConfig.Instance.MaxParticles / _fallbackParticleLimits.Count;

                foreach (ParticleSystem particleSystem in _fallbackParticleLimits.Keys)
                {
                    ParticleSystem.MainModule mainModule = particleSystem.main;

                    if (mainModule.maxParticles > maxPerSystem)
                        mainModule.maxParticles = maxPerSystem;
                }
                return;
            }

            // Change non-cached
            ParticleSystem[] particleSystems = Avatar.GetComponentsInChildren<ParticleSystem>(true);

            if (particleSystems.Length < 1)
                return;
            maxPerSystem = AntiCrasherConfig.Instance.MaxParticles / particleSystems.Length;

            foreach(ParticleSystem particleSystem in particleSystems)
            {
                ParticleSystem.MainModule mainModule = particleSystem.main;

                if (!_fallbackParticleLimits.ContainsKey(particleSystem))
                    _fallbackParticleLimits.Add(particleSystem, mainModule.maxParticles);

                if (mainModule.maxParticles > maxPerSystem)
                    mainModule.maxParticles = maxPerSystem;
            }
        }
        public void RestoreParticleLimits()
        {
            if (ModPrefs.GetBool("vrcextended", "antiCrasher"))
                return;
            if (_fallbackParticleLimits.Count < 1)
                return;

            foreach(ParticleSystem particleSystem in _fallbackParticleLimits.Keys)
            {
                ParticleSystem.MainModule mainModule = particleSystem.main;

                mainModule.maxParticles = _fallbackParticleLimits[particleSystem];
            }
        }

        public void RemoveCrashMesh()
        {
            if (!ModPrefs.GetBool("vrcextended", "antiCrasher"))
                return;

            // Change cached
            if(_fallbackMeshes.Count > 0)
            {
                foreach(MeshRenderer mesh in _fallbackMeshes.Keys)
                {
                    mesh.enabled = false;
                    ExtendedLogger.LogWarning("Removed crasher mesh from " + APIUser.displayName);
                }
                return;
            }

            // Change non-cached
            MeshRenderer[] meshes = Avatar.GetComponentsInChildren<MeshRenderer>();

            foreach(MeshRenderer mesh in meshes)
            {
                MeshFilter filter = mesh.GetComponent<MeshFilter>();

                if (filter == null)
                    continue;
                List<int> triangles = new List<int>();
                uint meshCount = 0;

                if(!filter.sharedMesh.isReadable)
                {
                    if (!_fallbackMeshes.ContainsKey(mesh))
                        _fallbackMeshes.Add(mesh, filter);

                    mesh.enabled = false;
                    ExtendedLogger.LogWarning("Removed unreadable mesh from " + APIUser.displayName);
                    continue;
                }
                for(int i = 0; i < filter.sharedMesh.subMeshCount; i++)
                {
                    filter.sharedMesh.GetTriangles(triangles, i);
                    meshCount += (uint)(triangles.Count / 3);
                    triangles.Clear();
                }

                if(meshCount > AntiCrasherConfig.Instance.MaxPolygons)
                {
                    if (!_fallbackMeshes.ContainsKey(mesh))
                        _fallbackMeshes.Add(mesh, filter);

                    mesh.enabled = false;
                    ExtendedLogger.LogWarning("Removed crasher mesh from " + APIUser.displayName);
                }
            }
        }
        public void RestoreCrashMesh()
        {
            if (ModPrefs.GetBool("vrcextended", "antiCrasher"))
                return;
            if (_fallbackMeshes.Count < 1)
                return;

            foreach(MeshRenderer mesh in _fallbackMeshes.Keys)
            {
                mesh.enabled = true;
                ExtendedLogger.LogWarning("Restored crasher mesh to " + APIUser.displayName);
            }
        }
        #endregion
    }
}

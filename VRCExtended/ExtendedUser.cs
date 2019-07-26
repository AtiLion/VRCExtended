using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRC;

using VRChat;

using VRCTools;

using UnityEngine;

using VRCExtended.GameScripts;

using VRC_AvatarDescriptor = VRCSDK2.VRC_AvatarDescriptor;

namespace VRCExtended
{
    internal class ExtendedUser : VRCEPlayer
    {
#if DEBUG
        #region Individual Volume Variables
        private float _volumeAvatar = 1f;
        private float _volumeVoice = 1f;
        #endregion
#endif

        #region AntiCrash Variables
        private static Shader _defaultShader;

        private Dictionary<Material, Shader> _fallbackShaders = new Dictionary<Material, Shader>();
        private Dictionary<ParticleSystem, int> _fallbackParticleLimits = new Dictionary<ParticleSystem, int>();
        private int _savedAllParticles = 0;
        private Dictionary<MeshRenderer, MeshFilter> _fallbackMeshes = new Dictionary<MeshRenderer, MeshFilter>();
        #endregion

        #region Local Colliders Variables
        private static FieldInfo fi_m_Direction = typeof(DynamicBoneCollider).GetField("m_Direction", BindingFlags.Public | BindingFlags.Instance);
        private bool _hasColliders = true;
        #endregion

        #region Local Colliders Properties
        public List<DynamicBoneCollider> BoneColliders { get; private set; } = new List<DynamicBoneCollider>();
        public List<DynamicBone> Bones { get; private set; } = new List<DynamicBone>();
        public LocalCollider Collider { get; private set; }
        public bool HasColliders
        {
            get => _hasColliders;
            set
            {
                _hasColliders = value;
                RemoveLocalColliders();
                OnAvatarCreated();
            }
        }
        #endregion

#if DEBUG
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
                AudioSource[] sources = Player.gameObject.GetComponentsInChildren<AudioSource>();

                foreach(AudioSource source in sources)
                {
                    if(source.name == "USpeak" || source.name == "Speaker")
                    {
                        ExtendedLogger.Log("Found " + source.name + " volume: " + source.volume);
                        source.volume = value;
                    }
                }
                _volumeVoice = value;
                /*if (Voice == null)
                    return;
                ExtendedLogger.Log("Test");
                Voice.volume = value;
                _volumeVoice = value;*/
            }
        }
        #endregion
#endif

        public ExtendedUser(VRCEPlayer player) : base(player.Player) => Setup();
        public ExtendedUser(Player player) : base(player) => Setup();
        public ExtendedUser() : base() => Setup();

        #region Setup Functions
        private void Setup()
        {
            AvatarManager.OnAvatarCreated += (GameObject avatar, VRC_AvatarDescriptor avatarDescriptor, bool _1) => OnAvatarCreated();

            ExtendedUser self = ExtendedServer.Users.FirstOrDefault(a => a.APIUser == APIUser);
            if(self != null)
            {
#if DEBUG
                _volumeAvatar = self._volumeAvatar;
                _volumeVoice = self._volumeVoice;
#endif

                _hasColliders = self._hasColliders;
                BoneColliders = self.BoneColliders;
                Bones = self.Bones;

                _fallbackShaders = self._fallbackShaders;
                _fallbackParticleLimits = self._fallbackParticleLimits;
                _fallbackMeshes = self._fallbackMeshes;
            }
        }
        #endregion

        #region Avatar Events
        internal void OnAvatarCreated()
        {
            if (Avatar == null)
                return;
#if DEBUG
            if (ModPrefs.GetBool("vrcextended", "userSpecificVolume"))
                VolumeAvatar = _volumeAvatar;
#endif
            if(ModPrefs.GetBool("vrcextended", "localColliders") && HasColliders)
            {
                foreach (ExtendedUser user in ExtendedServer.Users)
                {
                    if (user.APIUser == APIUser)
                        continue;

                    if (ModPrefs.GetBool("vrcextended", "multiLocalColliders") || (ModPrefs.GetBool("vrcextended", "selfLocalColliders") && IsSelf) || user.APIUser.id == Instance.APIUser.id)
                        foreach (DynamicBone bone in user.Bones)
                            foreach (DynamicBoneCollider collider in BoneColliders)
                                if (bone.m_Colliders.Contains(collider))
                                    bone.m_Colliders.Remove(collider);
                }
                BoneColliders.Clear();
                Bones.Clear();

                if ((ModPrefs.GetBool("vrcextended", "fakeColliders") && IsSelf) || ModPrefs.GetBool("vrcextended", "fakeCollidersOthers"))
                {
                    if (Animator != null)
                    {
                        Transform handbone_left = Animator.GetBoneTransform(HumanBodyBones.LeftHand);
                        Transform handbone_right = Animator.GetBoneTransform(HumanBodyBones.RightHand);

                        if (handbone_left != null && handbone_right != null)
                        {
                            if (handbone_left.GetComponent<DynamicBoneCollider>() == null && handbone_right.GetComponent<DynamicBoneCollider>() == null)
                            {
                                Transform fingerbone_left = Animator.GetBoneTransform(HumanBodyBones.LeftMiddleDistal);
                                Transform fingerbone_right = Animator.GetBoneTransform(HumanBodyBones.RightMiddleDistal);
                                Transform thumbbone = Animator.GetBoneTransform(HumanBodyBones.LeftThumbProximal);
                                Transform pinkybone = Animator.GetBoneTransform(HumanBodyBones.RightRingProximal);
                                float distance_left = 0.0016f;
                                float distance_right = 0.0016f;
                                float distance_hand = 0.0006f;
                                float power = (float)Math.Pow(10, 2);

                                if (thumbbone != null && pinkybone != null)
                                {
                                    distance_hand = (Mathf.Floor(Vector3.Distance(thumbbone.position, pinkybone.position) * power) / power) / 1000f;
                                    distance_hand += distance_hand / 4f;
                                }
                                if (fingerbone_left != null)
                                    distance_left = ((Mathf.Floor(Vector3.Distance(handbone_left.position, fingerbone_left.position) * power) / power) / 100f) + (distance_hand * 4f);
                                if (fingerbone_right != null)
                                    distance_right = ((Mathf.Floor(Vector3.Distance(handbone_right.position, fingerbone_right.position) * power) / power) / 100f) + (distance_hand * 4f);
                                ExtendedLogger.Log("Collider stats: " + distance_left + ", " + distance_right + ", " + distance_hand);
                                DynamicBoneCollider handcollider_left = handbone_left.gameObject.AddComponent<DynamicBoneCollider>();
                                DynamicBoneCollider handcollider_right = handbone_right.gameObject.AddComponent<DynamicBoneCollider>();
                                //GameObject show_left = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                                //GameObject show_right = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                                handcollider_left.m_Radius = distance_hand; // 0.0006f
                                handcollider_left.m_Height = distance_left;
                                handcollider_left.m_Center = new Vector3(0f, 0f, 0f);
                                fi_m_Direction.SetValue(handcollider_left, 1);
                                handcollider_left.m_Bound = 0;
                                /*show_left.transform.localScale = new Vector3(0.006f, 0.006f, 0.0006f);
                                show_left.transform.position = handbone_left.position;
                                show_left.transform.SetParent(handbone_left);*/


                                handcollider_right.m_Radius = distance_hand; // 0.0006f
                                handcollider_right.m_Height = distance_right;
                                handcollider_right.m_Center = new Vector3(0f, 0f, 0f);
                                fi_m_Direction.SetValue(handcollider_right, 1);
                                handcollider_right.m_Bound = 0;
                                /*show_right.transform.localScale = new Vector3(0.006f, 0.006f, 0.0006f);
                                show_right.transform.position = handbone_right.position;
                                show_right.transform.SetParent(handbone_right);*/

                                BoneColliders.Add(handcollider_left);
                                BoneColliders.Add(handcollider_right);
                                ExtendedLogger.Log("Added fake colliders to " + APIUser.displayName);
                            }
                        }
                    }
                }

                if (ModPrefs.GetBool("vrcextended", "smartColliders"))
                {
                    if (Collider != null)
                        GameObject.Destroy(Collider);

                    Transform handbone_left = Animator.GetBoneTransform(HumanBodyBones.LeftHand);
                    Transform handbone_right = Animator.GetBoneTransform(HumanBodyBones.RightHand);

                    if (handbone_left != null && handbone_right != null && IsSelf)
                    {
                        SphereCollider collider_left = handbone_left.gameObject.GetOrAddComponent<SphereCollider>();
                        SphereCollider collider_right = handbone_right.gameObject.GetOrAddComponent<SphereCollider>();

                        collider_left.tag = "handCollider";
                        collider_left.radius = 0.002f;
                        collider_left.enabled = true;
                        collider_left.isTrigger = true;

                        collider_right.tag = "handCollider";
                        collider_right.radius = 0.002f;
                        collider_right.enabled = true;
                        collider_right.isTrigger = true;
                    }
                    CapsuleCollider aviCollider = Avatar.GetOrAddComponent<CapsuleCollider>();

                    aviCollider.tag = "player";
                    aviCollider.height = Avatar.transform.localScale.y;
                    aviCollider.radius = Avatar.transform.localScale.x;
                    aviCollider.enabled = true;
                    aviCollider.isTrigger = true;

                    Collider = Avatar.GetOrAddComponent<LocalCollider>();
                }
                else
                {
                    if (ModPrefs.GetBool("vrcextended", "targetHandColliders") && Animator != null)
                    {
                        Transform handbone_left = Animator.GetBoneTransform(HumanBodyBones.LeftHand);
                        Transform handbone_right = Animator.GetBoneTransform(HumanBodyBones.RightHand);

                        if (handbone_left != null && handbone_right != null) // Can't be too sure
                        {
                            BoneColliders.AddRange(handbone_left.GetComponentsInChildren<DynamicBoneCollider>(true));
                            BoneColliders.AddRange(handbone_right.GetComponentsInChildren<DynamicBoneCollider>(true));
                        }
                    }
                    else
                        BoneColliders.AddRange(Avatar.GetComponentsInChildren<DynamicBoneCollider>(true).Where(a => !ModPrefs.GetBool("vrcextended", "ignoreInsideColliders") || (int)a.m_Bound != 1));
                    Bones.AddRange(Avatar.GetComponentsInChildren<DynamicBone>(true).Where(a => a.m_Colliders.Count > 1));

                    foreach (ExtendedUser user in ExtendedServer.Users)
                    {
                        if (user.APIUser == APIUser)
                            continue;

                        if (ModPrefs.GetBool("vrcextended", "multiLocalColliders") || (ModPrefs.GetBool("vrcextended", "selfLocalColliders") && IsSelf) || user.APIUser == Instance.APIUser)
                            foreach (DynamicBone bone in Bones)
                                foreach (DynamicBoneCollider collider in user.BoneColliders)
                                    bone.m_Colliders.Add(collider);
                        if (ModPrefs.GetBool("vrcextended", "multiLocalColliders") || (ModPrefs.GetBool("vrcextended", "selfLocalColliders") && user.APIUser == Instance.APIUser) || IsSelf)
                            foreach (DynamicBone bone in user.Bones)
                                foreach (DynamicBoneCollider collider in BoneColliders)
                                    bone.m_Colliders.Add(collider);
                    }
                }
                ExtendedLogger.Log("Added local colliders to " + APIUser.displayName + "!");
            }
            if(ModPrefs.GetBool("vrcextended", "antiCrasher"))
            {
                _fallbackShaders.Clear();
                RemoveCrashShaders();

                _savedAllParticles = 0;
                _fallbackParticleLimits.Clear();
                LimitParticles();

                _fallbackMeshes.Clear();
                RemoveCrashMesh();
            }
        }
        #endregion

        #region LocalColliders Functions
        public void RemoveLocalColliders()
        {
            foreach(DynamicBone bone in Bones)
                foreach(DynamicBoneCollider collider in bone.m_Colliders.ToArray())
                    if (!BoneColliders.Contains(collider))
                        bone.m_Colliders.Remove(collider);
            if (Collider != null)
                GameObject.Destroy(Collider);
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

            // Change cached
            if(_fallbackParticleLimits.Count > 0)
            {
                foreach(ParticleSystem particleSystem in _fallbackParticleLimits.Keys)
                {
                    ParticleSystem.MainModule mainModule = particleSystem.main;
                    float percantage = (float)mainModule.maxParticles / _savedAllParticles;
                    int maxParticles = (int)Math.Floor(percantage * AntiCrasherConfig.Instance.MaxParticles);

                    if (mainModule.maxParticles > maxParticles)
                        mainModule.maxParticles = maxParticles;
                }
            }

            // Change non-cached
            ParticleSystem[] particleSystems = Avatar.GetComponentsInChildren<ParticleSystem>(true);

            if (particleSystems.Length < 1)
                return;
            particleSystems.All(a => { _savedAllParticles += a.main.maxParticles; return true; });
            foreach(ParticleSystem particleSystem in particleSystems)
            {
                ParticleSystem.MainModule mainModule = particleSystem.main;
                float percantage = (float)mainModule.maxParticles / _savedAllParticles;
                int maxParticles = (int)Math.Floor(percantage * AntiCrasherConfig.Instance.MaxParticles);

                if (!_fallbackParticleLimits.ContainsKey(particleSystem))
                    _fallbackParticleLimits.Add(particleSystem, mainModule.maxParticles);

                if (mainModule.maxParticles > maxParticles)
                    mainModule.maxParticles = maxParticles;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using VRC;
using VRC_AvatarDescriptor = VRCSDK2.VRC_AvatarDescriptor;

using VRCExtended.VRChat;
using VRCExtended.Config;

namespace VRCExtended.Modules.LocalColliders
{
    internal class ColliderHandler : MonoBehaviour
    {
        #region Config Variables
        private ConfigLocalColliders config;

        private static float timer = 0f;
        private static float waitTime = 1f;
        private static float maxDistance = Mathf.Sqrt(2f);
        #endregion

        #region User Properties
        public VRCEPlayer Player { get; private set; }
        #endregion

        #region Unity Functions
        void Awake()
        {
            // Get config
            config = ManagerConfig.Config.LocalColliders;

            // Get Player
            Player = new VRCEPlayer(gameObject.GetComponent<Player>());

            // Setup events
            Player.AvatarManager.OnAvatarCreated += OnAvatarCreated;
        }

        void Update()
        {
            if (config == null || !(bool)config.DisableOnDistance || !(bool)config.Enabled)
                return;

            // Check timing
            timer += Time.deltaTime;
            if (timer < waitTime)
                return;
            timer -= waitTime;

            // Setup for distance check
            Vector3 position = gameObject.transform.position;

            // Set colliders
            foreach(ColliderHandler handler in ColliderController.Users.Values)
            {
                if (handler == this)
                    continue;
                if((handler.transform.position - position).sqrMagnitude < maxDistance)
                {
                    if (handler.Active)
                        continue;

                    // Add the colliders
                    if((bool)config.PlayersInteractWithOthers)
                    {
                        AddColliderToBones(handler.Colliders);
                        handler.AddColliderToBones(Colliders);
                    }
                    else if(handler.Player == VRCEPlayer.Instance)
                    {
                        AddColliderToBones(handler.Colliders);
                        if ((bool)config.PlayersInteractWithMe)
                            handler.AddColliderToBones(Colliders);
                    }
                    handler.Active = true;
                }
                else
                {
                    if (!handler.Active)
                        continue;

                    // Remove the colliders
                    if ((bool)config.PlayersInteractWithOthers)
                    {
                        RemoveColliderFromBones(handler.Colliders);
                        handler.RemoveColliderFromBones(Colliders);
                    }
                    else if (handler.Player == VRCEPlayer.Instance)
                    {
                        RemoveColliderFromBones(handler.Colliders);
                        if ((bool)config.PlayersInteractWithMe)
                            handler.RemoveColliderFromBones(Colliders);
                    }
                    handler.Active = false;
                }
            }
        }
        #endregion

        #region Collider Variables
        private bool Active = false;
        private List<DynamicBone> Bones = new List<DynamicBone>();
        private List<DynamicBoneCollider> Colliders = new List<DynamicBoneCollider>();
        #endregion
        #region Collider Functions
        public IEnumerable<DynamicBoneCollider> GetColliders()
        {
            if (!(bool)config.PlayersInteractWithMe && !(bool)config.PlayersInteractWithOthers && Player != VRCEPlayer.Instance)
                return new DynamicBoneCollider[0];
            List<DynamicBoneCollider> colliders = new List<DynamicBoneCollider>();

            if ((bool)config.FakeCollidersOthers || ((bool)config.FakeCollidersMe && Player == VRCEPlayer.Instance))
            {
                // Get hands
                Transform handbone_left = Player.Animator.GetBoneTransform(HumanBodyBones.LeftHand);
                Transform handbone_right = Player.Animator.GetBoneTransform(HumanBodyBones.RightHand);

                if(handbone_left.GetComponent<DynamicBoneCollider>() == null || handbone_right.GetComponent<DynamicBoneCollider>() == null)
                {
                    Transform middleFingerProx = Player.Animator.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);
                    Transform middleFingerDist = Player.Animator.GetBoneTransform(HumanBodyBones.LeftMiddleDistal);
                    DynamicBoneCollider handcollider_left = handbone_left.gameObject.AddComponent<DynamicBoneCollider>();
                    DynamicBoneCollider handcollider_right = handbone_right.gameObject.AddComponent<DynamicBoneCollider>();
                    float size = Vector3.Distance(middleFingerProx.position, middleFingerDist.position);

                    handcollider_left.m_Radius = size;
                    handcollider_left.m_Center = new Vector3(0.005f, size, 0f);
                    handcollider_left.m_Bound = 0;

                    handcollider_right.m_Radius = size;
                    handcollider_right.m_Center = new Vector3(0.005f, size, 0f);
                    handcollider_right.m_Bound = 0;

                    colliders.Add(handcollider_left);
                    colliders.Add(handcollider_right);
                    ExtendedLogger.Log($"Added fake colliders to {Player.DisplayName}!");
                    return colliders;
                }
            }

            if ((bool)config.EnableForHandsOnly && Player.Animator != null)
            {
                int oldNum = colliders.Count;

                // Hand colliders
                Transform handbone_left = Player.Animator.GetBoneTransform(HumanBodyBones.LeftHand);
                Transform handbone_right = Player.Animator.GetBoneTransform(HumanBodyBones.RightHand);
                if (handbone_left != null && handbone_right != null)
                {
                    colliders.AddRange(handbone_left.GetComponentsInChildren<DynamicBoneCollider>(true).Where(a => (int)a.m_Bound != 1));
                    colliders.AddRange(handbone_right.GetComponentsInChildren<DynamicBoneCollider>(true).Where(a => (int)a.m_Bound != 1));
                }

                // Leg colliders
                if ((bool)config.EnableForLegs)
                {
                    Transform footbone_left = Player.Animator.GetBoneTransform(HumanBodyBones.LeftFoot);
                    Transform footbone_right = Player.Animator.GetBoneTransform(HumanBodyBones.RightFoot);
                    if (footbone_left != null && footbone_right != null)
                    {
                        colliders.AddRange(footbone_left.GetComponentsInChildren<DynamicBoneCollider>(true).Where(a => (int)a.m_Bound != 1));
                        colliders.AddRange(footbone_right.GetComponentsInChildren<DynamicBoneCollider>(true).Where(a => (int)a.m_Bound != 1));
                    }
                }

                return colliders;
            }
            else if (!(bool)config.EnableForHandsOnly)
                return Player.Avatar.GetComponentsInChildren<DynamicBoneCollider>(true).Where(a => (int)a.m_Bound != 1);
            return new DynamicBoneCollider[0];
        }
        public void RemoveAllColliders()
        {
            foreach (ColliderHandler handler in ColliderController.Users.Values)
                handler.RemoveColliderFromBones(Colliders);
        }
        public void ClearColliders()
        {

            RemoveAllColliders();
            Bones.Clear();
            Colliders.Clear();
        }
        public void PopulateColliders()
        {
            // Clear old colliders
            ClearColliders();

            // Populate colliders
            Colliders.AddRange(GetColliders());
            Bones.AddRange(Player.Avatar.GetComponentsInChildren<DynamicBone>(true));
        }

        public void RemoveColliderFromBones(DynamicBoneCollider collider)
        {
            foreach(DynamicBone bone in Bones)
                if (bone.m_Colliders.Contains(collider))
                    bone.m_Colliders.Remove(collider);
        }
        public void RemoveColliderFromBones(IEnumerable<DynamicBoneCollider> colliders)
        {
            foreach (DynamicBoneCollider collider in colliders)
                RemoveColliderFromBones(collider);
        }

        public void AddColliderToBones(DynamicBoneCollider collider)
        {
            foreach (DynamicBone bone in Bones)
                if (!bone.m_Colliders.Contains(collider))
                    bone.m_Colliders.Add(collider);
        }
        public void AddColliderToBones(IEnumerable<DynamicBoneCollider> colliders)
        {
            foreach (DynamicBoneCollider collider in colliders)
                AddColliderToBones(collider);
        }

        public void ApplyColliders()
        {
            if (Player == VRCEPlayer.Instance)
                return;
            if((bool)config.PlayersInteractWithOthers)
            {
                foreach(ColliderHandler handler in ColliderController.Users.Values)
                {
                    if (handler == this)
                        continue;

                    AddColliderToBones(handler.Colliders);
                    handler.AddColliderToBones(Colliders);
                }
            }
            else
            {
                ColliderHandler handler = ColliderController.Users[VRCEPlayer.Instance.UniqueID];

                AddColliderToBones(handler.Colliders);
                if ((bool)config.PlayersInteractWithMe)
                    handler.AddColliderToBones(Colliders);
            }
        }
        #endregion

        #region Avatar Event Handlers
        private void OnAvatarCreated(GameObject avatar, VRC_AvatarDescriptor descriptor, bool loaded)
        {
            if (!(bool)config.Enabled)
                return;

            PopulateColliders();
            if (!(bool)config.DisableOnDistance)
                ApplyColliders();

            ExtendedLogger.Log($"Local colliders setup for {Player.DisplayName}");
        }
        #endregion
    }
}

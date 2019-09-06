using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using VRC;

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
        private static float maxDistance = 3f;
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
            Player.onAvatarIsReady += Player_onAvatarIsReady;
        }

        void FixedUpdate()
        {
            if (!(bool)config.DisableOnDistance)
                return;

            // Check timing
            timer += Time.deltaTime;
            if (timer < waitTime)
                return;
            timer -= waitTime;

            // Setup for distance check
            Vector3 position = gameObject.transform.position;
            float maxDistance = ColliderHandler.maxDistance * ColliderHandler.maxDistance; // Faster than Pow

            // Set colliders
            foreach(ColliderHandler handler in ColliderController.Users.Values)
            {
                if (handler == this)
                    continue;
            }
        }
        #endregion

        #region Collider Variables
        private List<DynamicBone> Bones = new List<DynamicBone>();
        private List<DynamicBoneCollider> Colliders = new List<DynamicBoneCollider>();
        #endregion
        #region Collider Functions
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
        #endregion

        #region Avatar Event Handlers
        private void Player_onAvatarIsReady()
        {
            // Clear old colliders
            foreach (ColliderHandler handler in ColliderController.Users.Values)
                handler.RemoveColliderFromBones(Colliders);
            Bones.Clear();
            Colliders.Clear();
        }
        #endregion
    }
}

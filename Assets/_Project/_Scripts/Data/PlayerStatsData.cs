using Sirenix.OdinInspector;
using UnityEngine;

namespace Pilk.Scripts.Data
{
    [CreateAssetMenu(fileName = nameof(PlayerStatsData), menuName = "Scriptable Objects/stats/PlayerStats")]
    public class PlayerStatsData : SerializedScriptableObject
    {
        [SerializeField]
        private float _movementSpeed;
        [SerializeField]
        private float _jumpHeight;
        [SerializeField]
        private float _jumpDistance;
        [SerializeField]
        private float _maxFallSpeed;
        [SerializeField] 
        private float _attackDistance;
        [SerializeField] 
        private float _attackSpeed;

        public float MovementSpeed => _movementSpeed;
        public float JumpHeight => _jumpHeight;
        public float JumpDistance => _jumpDistance;
        public float MaxFallSpeed => _maxFallSpeed;
        public float AttackDistance => _attackDistance;
        public float AttackSpeed => _attackSpeed;
    }
}

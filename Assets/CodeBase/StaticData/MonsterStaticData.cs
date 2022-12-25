using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CodeBase.StaticData
{
    [CreateAssetMenu(fileName = "MonsterData", menuName = "StaticData/Monster", order = 51)]
    public class MonsterStaticData : ScriptableObject
    {
        public MonsterTypeId MonsterTypeId;

        public int MinLoot = 1;
        public int MaxLoot = 5; 
        
        [Range(1, 100)] public int Hp;
        [Range(1f, 30f)] public float Damage;
        [Range(1f, 5f)] public float AttackCooldown = 3f;
        [Range(0.5f, 1f)] public float EffectiveDistance;
        [Range(0.5f, 1f)] public float Cleavage;
        
        public AssetReferenceGameObject PrefabReference;
    }
}
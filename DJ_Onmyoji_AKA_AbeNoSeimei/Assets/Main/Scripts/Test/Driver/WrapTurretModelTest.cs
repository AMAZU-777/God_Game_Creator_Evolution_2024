using System.Collections;
using System.Collections.Generic;
using Main.Common;
using Main.Model;
using Main.Utility;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Main.Test.Driver
{
    public class WrapTurretModelTest : MonoBehaviour
    {
        // [SerializeField] private PentagramTurnTableModel pentagramTurnTableModel;
        [SerializeField] private WrapTurretModel wrapTurretModel;
        [SerializeField] private Transform objectsPoolPrefab;
        private ObjectsPoolModel _objectsPoolModel = null;
        private OnmyoBulletConfig _onmyoBulletConfig;
        private void Reset()
        {
            // pentagramTurnTableModel = GameObject.Find("PentagramTurnTable").GetComponent<PentagramTurnTableModel>();
            wrapTurretModel = GameObject.Find("WrapTurret").GetComponent<WrapTurretModel>();
        }
        // Start is called before the first frame update
        void Start()
        {
            var spawnUtility = new SpawnUtility();
            this.UpdateAsObservable()
                .Select(_ => spawnUtility.FindOrInstantiateForGetObjectsPoolModel(objectsPoolPrefab))
                .Where(x => x != null)
                .Take(1)
                .Subscribe(x =>
                {
                    _objectsPoolModel = x;
                });
            var stubOnmyoBulletConfig = GetComponent<Stub.WrapTurretModelTest>().OnmyoBulletConfig;
            // // 広範囲（爆発）
            // _onmyoBulletConfig = new OnmyoBulletConfig()
            // {
            //     // moveDirection = stubOnmyoBulletConfig.moveDirection,
            //     // moveSpeed = stubOnmyoBulletConfig.moveSpeed,
            //     // actionRate = stubOnmyoBulletConfig.actionRate,
            //     bulletLifeTime = stubOnmyoBulletConfig.bulletLifeTime,
            //     // attackPoint = stubOnmyoBulletConfig.attackPoint,
            //     // range = stubOnmyoBulletConfig.range,
            //     // trackingOfAny = stubOnmyoBulletConfig.trackingOfAny,
            //     // debuffEffectLifeTime = stubOnmyoBulletConfig.debuffEffectLifeTime,
            //     onmyoBulletConfigOfExplosion = new OnmyoBulletConfigOfExplosion()
            //     {
            //         explosionDuration = stubOnmyoBulletConfig.onmyoBulletConfigOfExplosion.explosionDuration,
            //         explosionRange = stubOnmyoBulletConfig.onmyoBulletConfigOfExplosion.explosionRange,
            //     },
            //     // continuousFire = new OnmyoBulletConfigOfHighEnd()
            //     // {
            //     //     actionRate = stubOnmyoBulletConfig.continuousFire.actionRate,
            //     //     instanceMax = stubOnmyoBulletConfig.continuousFire.instanceMax,
            //     //     spreadingAngle = stubOnmyoBulletConfig.continuousFire.spreadingAngle,
            //     // },
            //     // penetrating = stubOnmyoBulletConfig.penetrating,
            //     // spreading = new OnmyoBulletConfigOfHighEnd()
            //     // {
            //     //     actionRate = stubOnmyoBulletConfig.spreading.actionRate,
            //     //     instanceMax = stubOnmyoBulletConfig.spreading.instanceMax,
            //     //     spreadingAngle = stubOnmyoBulletConfig.spreading.spreadingAngle,
            //     // },
            //     // knockBack = stubOnmyoBulletConfig.knockBack,
            //     // paralysis = stubOnmyoBulletConfig.paralysis,
            //     // largeRange = stubOnmyoBulletConfig.largeRange,
            //     // propagation = stubOnmyoBulletConfig.propagation,
            //     // increasedDamage = stubOnmyoBulletConfig.increasedDamage,
            //     // poison = stubOnmyoBulletConfig.poison,
            //     // cancelBuff = stubOnmyoBulletConfig.cancelBuff,
            //     // drain = stubOnmyoBulletConfig.drain,
            // };
            // 連射／拡散
            _onmyoBulletConfig = new OnmyoBulletConfig()
            {
                // moveDirection = stubOnmyoBulletConfig.moveDirection,
                // moveSpeed = stubOnmyoBulletConfig.moveSpeed,
                // actionRate = stubOnmyoBulletConfig.actionRate,
                bulletLifeTime = stubOnmyoBulletConfig.bulletLifeTime,
                // attackPoint = stubOnmyoBulletConfig.attackPoint,
                // range = stubOnmyoBulletConfig.range,
                // trackingOfAny = stubOnmyoBulletConfig.trackingOfAny,
                // debuffEffectLifeTime = stubOnmyoBulletConfig.debuffEffectLifeTime,
                // onmyoBulletConfigOfExplosion = new OnmyoBulletConfigOfExplosion()
                // {
                //     explosionDuration = stubOnmyoBulletConfig.onmyoBulletConfigOfExplosion.explosionDuration,
                //     explosionRange = stubOnmyoBulletConfig.onmyoBulletConfigOfExplosion.explosionRange,
                // },
                continuousFire = new OnmyoBulletConfigOfHighEnd()
                {
                    actionRate = stubOnmyoBulletConfig.continuousFire.actionRate,
                    instanceMax = stubOnmyoBulletConfig.continuousFire.instanceMax,
                    spreadingAngle = stubOnmyoBulletConfig.continuousFire.spreadingAngle,
                },
                // penetrating = stubOnmyoBulletConfig.penetrating,
                // spreading = new OnmyoBulletConfigOfHighEnd()
                // {
                //     actionRate = stubOnmyoBulletConfig.spreading.actionRate,
                //     instanceMax = stubOnmyoBulletConfig.spreading.instanceMax,
                //     spreadingAngle = stubOnmyoBulletConfig.spreading.spreadingAngle,
                // },
                // knockBack = stubOnmyoBulletConfig.knockBack,
                // paralysis = stubOnmyoBulletConfig.paralysis,
                // largeRange = stubOnmyoBulletConfig.largeRange,
                // propagation = stubOnmyoBulletConfig.propagation,
                // increasedDamage = stubOnmyoBulletConfig.increasedDamage,
                // poison = stubOnmyoBulletConfig.poison,
                // cancelBuff = stubOnmyoBulletConfig.cancelBuff,
                // drain = stubOnmyoBulletConfig.drain,
            };
        }

        void OnGUI()
        {
            if (GUILayout.Button("WrapTurretModelのテストを開始"))
            {
                if (wrapTurretModel != null)
                {
                    // var onmyoBulletConfig = ((IWrapTurretModelTest)pentagramTurnTableModel.WrapTurretModel).InitializeOnmyoBulletConfig();
                    if (!((IWrapTurretModelTest)wrapTurretModel).ActionOfBullet(_objectsPoolModel, _onmyoBulletConfig))
                        Debug.LogError("ActionOfBullet");
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <see cref="Main.Model.WrapTurretModel"/>
    public interface IWrapTurretModelTest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <see cref="Main.Model.WrapTurretModel.InitializeOnmyoBulletConfig"/>
        public OnmyoBulletConfig InitializeOnmyoBulletConfig();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectsPoolModel"></param>
        /// <param name="onmyoBulletConfig"></param>
        /// <returns></returns>
        /// <see cref="Main.Model.WrapTurretModel.ActionOfBullet(ObjectsPoolModel, OnmyoBulletConfig)"/>
        public bool ActionOfBullet(ObjectsPoolModel objectsPoolModel, OnmyoBulletConfig onmyoBulletConfig);
    }
}

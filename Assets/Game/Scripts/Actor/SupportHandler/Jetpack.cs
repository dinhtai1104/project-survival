using System;
using UnityEngine;

namespace Game.GameActor
{

    [CreateAssetMenu (menuName ="MoveAddOn/Jetpack")]
    public class Jetpack : MoveAddOn
    {
        // 10 fuel in 1 sec
        [Header ("fuel per second")]
        public  ValueConfigSearch FuelConsumeRate ;
        public bool JetPack => maxFuel > 0;

        public ValueConfigSearch MaxFuel ;

        private float maxFuel,fuelConsumeRate;

        public Vector2 JetpackForce;



        float currentFuel = 0;
        bool isJetpackTriggered = false;
        JetpackFuelUI ui;

        public override MoveAddOn SetUp(MoveHandler handler)
        {
            Jetpack instane= (Jetpack)base.SetUp(handler);
            instane.fuelConsumeRate = FuelConsumeRate.FloatValue;
            instane.maxFuel = MaxFuel.FloatValue;
            instane.currentFuel = MaxFuel.FloatValue;
            instane.JetpackForce = JetpackForce;
            Game.Pool.GameObjectSpawner.Instance.Get("JetpackFuelUI",obj=> 
            {
                instane.ui = obj.GetComponent<JetpackFuelUI>();
                instane.ui.SetUp(handler.Character.GetTransform(), new Vector3(0, -0.5f));
            }, poolType: Pool.EPool.Pernament);
            return instane;
        }
        public override void OnFixedUpdate(MoveHandler handler)
        {
            HandleJetpack(handler);
        }

        public override void Init(MoveHandler handler)
        {
            handler.onCharacterLanded += OnCharacterLanded;
            handler.onCharacterJumpReleased += OnCharacterJumpReleased;

            Messenger.AddListener(EventKey.PlayerTeleported, OnGameClear);
        }

        private void OnGameClear()
        {
            if (ui != null)
            {
                ui.Hide();
            }
        }

        private void OnCharacterJumpReleased(MoveHandler moveHandler)
        {
            if (JetPack)
            {
                isJetpackTriggered = false;
                Messenger.Broadcast(EventKey.JetPackReleased);

            }
        }

        private void OnCharacterLanded(MoveHandler moveHandler)
        {
            currentFuel = maxFuel;
            if (ui != null)
            {
                ui.Hide();

            }

        }

        public override void OnUpdate(MoveHandler handler)
        {
        }

        void HandleJetpack(MoveHandler moveHandler)
        {
            if (JetPack)
            {
                if (!moveHandler.isGrounded)
                {
                    if (JetPack && !moveHandler.isClimbing && moveHandler.isHoldingJump && !moveHandler.isFallingFromLastJump && moveHandler.Character.GetRigidbody().velocity.y < 0)
                    {
                        moveHandler.isFallingDown = false;
                        moveHandler.isFallingFromLastJump = true;
                        
                        ui.Show();
                        isJetpackTriggered = true;
                        moveHandler.Character.GetRigidbody().velocity = new Vector2(moveHandler.Character.GetRigidbody().velocity.x, 0);

                        Messenger.Broadcast(EventKey.JetPackTriggered,moveHandler.Character);
                    }
                }
                if (isJetpackTriggered && currentFuel > 0)
                {
                    moveHandler.Character.GetRigidbody().AddForce(JetpackForce, ForceMode2D.Force);
                    currentFuel -= fuelConsumeRate * Time.fixedDeltaTime;
                    ui.UpdateFuel(currentFuel, maxFuel);

                }
            }
        }
    }
}
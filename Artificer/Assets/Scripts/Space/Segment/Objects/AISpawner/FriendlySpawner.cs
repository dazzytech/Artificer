using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Data.Space;
using Data.Shared;
using Space.Contract;

namespace Space.Generator
{
    public class FriendlySpawner : SpawnerBehaviour
    {
        /*public static ShipGenerator ShipGen;
        public ShipData ShipPending;
        public Transform Target;
        public string SpawnType;

        public override void Trigger()
        {
            Transform newShip = null;

            Vector3 position = new Vector3
                (transform.position.x + Random.Range(-10, 10),
                 transform.position.y + Random.Range(-10, 10));

            newShip = ShipGen.GenerateShip
                (ShipPending, position,
                 new Vector2(Random.Range(-1f, 1f),
                            Random.Range(-1f, 1f)));

            newShip.name = "Friendly";

            switch(SpawnType)
            {
                case "cargo":
                {
                    SimpleFriendlyCargoController cargo = 
                        newShip.gameObject.AddComponent<SimpleFriendlyCargoController>();
                    cargo.SetController
                        (newShip.gameObject.GetComponent<ShipMessegeController>());
                    cargo.target = Target;
                    cargo.SetAlignment();
                    break;
                }
                case "guard":
                {
                     SimpleFriendlyGuardController patrol = 
                            newShip.gameObject.AddComponent<SimpleFriendlyGuardController>();
                     patrol.SetController
                            (newShip.gameObject.GetComponent<ShipMessegeController>());
                     patrol.SetAlignment();
                     break;
                }
                default:
                {
                    SimpleFriendlyController patrol = 
                            newShip.gameObject.AddComponent<SimpleFriendlyController>();
                    patrol.SetController
                            (newShip.gameObject.GetComponent<ShipMessegeController>());
                    patrol.follow = Target;
                    patrol.SetAlignment();
                    break;
                 }
            }
                
            GameObject.Find("_gui").
                    SendMessage("AddUIPiece", newShip, 
                                SendMessageOptions.DontRequireReceiver);

            SegmentObjectBehaviour obj = newShip.gameObject.AddComponent<SegmentObjectBehaviour>();
            //obj.Create(300f);

            FriendlySpawnManager.AddShip(newShip);
            Disengage();
        }*/
    }

}
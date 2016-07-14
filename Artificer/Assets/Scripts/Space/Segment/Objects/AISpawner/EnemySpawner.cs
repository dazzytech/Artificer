
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Data.Space;
using Data.Shared;
using Space.Contract;

namespace Space.Generator
{
    public class EnemySpawner : SpawnerBehaviour
    {
        /*public static ShipGenerator ShipGen;
        public Transform Target;
        public ShipData ShipPending;

        public override void Trigger()
        {
            GameObject newShip = null;
                    
            Vector3 position = new Vector3
                    (transform.position.x + Random.Range(-10, 10),
                     transform.position.y + Random.Range(-10, 10));

            newShip = ShipGen.GenerateShip
                        (ShipPending, position,
                new Vector2(Random.Range(-1f, 1f),
                                Random.Range(-1f, 1f)));
                    
            newShip.name = "Enemy";
            SimpleEnemyController raider =
                newShip.AddComponent<SimpleEnemyController>();
            raider.SetController
                (newShip.GetComponent<ShipMessegeController>());
            SegmentObjectBehaviour obj = raider.gameObject.AddComponent<SegmentObjectBehaviour>();
            //obj.Create(300f);
            raider.target = Target;
            raider.SetAlignment();
                    
            if(GameObject.Find("_gui") != null)
               GameObject.Find("_gui").
                   SendMessage("AddUIPiece", newShip, 
                   SendMessageOptions.DontRequireReceiver);

            EnemySpawnManager.AddShip(newShip);

            Disengage();
        }*/
    }
    
}
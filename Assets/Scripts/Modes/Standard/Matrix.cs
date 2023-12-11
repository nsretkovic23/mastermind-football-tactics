using System.Collections;
using System.Collections.Generic;
using Base;
using UnityEngine;

namespace Modes.Standard
{
    public class Matrix : Base.Matrix
    {
        // Start is called before the first frame update
        private void Awake()
        {
            Debug.Log(matrix);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override List<Base.Field> GetAvailableFieldsToMove(Player player)
        {
            throw new System.NotImplementedException();
        }

        public override List<Base.Field> GetAvailableFieldsToPass(Player player)
        {
            throw new System.NotImplementedException();
        }

        public override List<Base.Field> GetAvailableFieldsToShoot(Player player)
        {
            throw new System.NotImplementedException();
        }
    }
}
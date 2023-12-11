using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base
{
    public abstract class Matrix : MonoBehaviour
    {
        [SerializeField] protected short rows;
        [SerializeField] protected short columns;
        [SerializeField] protected List<Field> fields;

        protected Field[,] matrix;

        public void InitializeMatrix()
        {
            matrix = new Field[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    matrix[i, j] = fields[i * columns + j];
                }
            }
        }

        public abstract List<Field> GetAvailableFieldsToMove(Player player);

        public abstract List<Field> GetAvailableFieldsToPass(Player player);

        public abstract List<Field> GetAvailableFieldsToShoot(Player player);

    }
}
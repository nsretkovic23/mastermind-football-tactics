using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
// ReSharper disable InconsistentNaming

namespace Base
{
    public abstract class Matrix : MonoBehaviour
    {
        public UnityAction<Field> OnFieldClicked { get; set; }
        [SerializeField] protected short rows;
        [SerializeField] protected short columns;
        
        [SerializeField] protected List<Field> fields;
        public List<Field> Fields => fields;
        
        protected Field[,] matrix;

        public void InitializeMatrix()
        {
            matrix = new Field[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    matrix[i, j] = fields[i * columns + j];
                    matrix[i,j].Position = new Vector2Int(i, j);
                    matrix[i,j].OnFieldClicked += FieldClickedHandler;
                }
            }
        }

        public void FieldClickedHandler(Field field)
        {
            OnFieldClicked?.Invoke(field);
        }

        public abstract List<Field> GetAvailableFieldsToMoveThePlayer(Field selectedField, Manager manager, Player player, Action action);

        public abstract List<Field> GetAvailableFieldsToPass(Field selectedField, Manager manager, Player player, Action action);

        public abstract List<Field> GetAvailableFieldsToShoot(Field selectedField, Manager manager, Action action);

        protected abstract List<Field> GetAvailableFieldsToPassFromList(List<Base.Field> directionFields,
            Base.Player player);
        
        /// <summary>
        /// Returns fields in order to the right of the field that is passed as a parameter
        /// It doesn't include goalkeeper fields and neither forbidden fields
        /// </summary>
        protected virtual List<Field> GetOrderedFieldsToTheRightWithoutGK(Base.Field field)
        {
            List<Base.Field> right = new ();
            for (int i = field.Y + 1; i < columns ; i++)
            {
                Base.Field f = matrix[field.X, i];
                if (!f.IsForbidden && !f.IsGoalkeeper)
                {
                    right.Add(f);
                }
            }

            return right;
        }
        
        protected virtual List<Field> GetOrderedFieldsToTheLeftWithoutGK(Base.Field field)
        {
            List<Base.Field> left = new ();
            for (int i = field.Y - 1; i >= 0; i--)
            {
                Base.Field f = matrix[field.X, i];
                if (!f.IsForbidden && !f.IsGoalkeeper)
                {
                    left.Add(f);
                }
            }

            return left;
        }
        
        protected virtual List<Field> GetOrderedFieldsUpwards(Base.Field field)
        {
            List<Base.Field> up = new ();
            for (int i = field.X - 1; i >= 0; i--)
            {
                Base.Field f = matrix[i, field.Y];
                if (!f.IsForbidden)
                {
                    up.Add(f);
                }
            }

            return up;
        }
        
        protected virtual List<Field> GetOrderedFieldsDownwards(Base.Field field)
        {
            List<Base.Field> down = new ();
            for (int i = field.X + 1; i < rows; i++)
            {
                Base.Field f = matrix[i, field.Y];
                if (!f.IsForbidden)
                {
                    down.Add(f);
                }
            }

            return down;
        }
        
        protected virtual List<Field> GetOrderedFieldsDiagonalDownRight(Base.Field field)
        {
            List<Base.Field> diagonalDownRight = new ();
            for (int i = field.X + 1, j = field.Y + 1;
                i < rows && j < columns - 1;
                i++, j++)
            {
                Base.Field f = matrix[i, j];
                if (!f.IsForbidden && !f.IsGoalkeeper)
                {
                    diagonalDownRight.Add(f);
                }
            }

            return diagonalDownRight;
        }
        
        protected virtual List<Field> GetOrderedFieldsDiagonalUpRight(Base.Field field)
        {
            List<Base.Field> diagonalUpRight = new ();
            for (int i = field.X - 1, j = field.Y + 1;
                i >= 0 && j < columns - 1;
                i--, j++)
            {
                Base.Field f = matrix[i, j];
                if (!f.IsForbidden && !f.IsGoalkeeper)
                {
                    diagonalUpRight.Add(f);
                }
            }

            return diagonalUpRight;
        }
        
        protected virtual List<Field> GetOrderedFieldsDiagonalDownLeft(Base.Field field)
        {
            List<Base.Field> diagonalDownLeft = new ();
            for (int i = field.X + 1, j = field.Y - 1;
                i < rows && j >= 0;
                i++, j--)
            {
                Base.Field f = matrix[i, j];
                if (!f.IsForbidden && !f.IsGoalkeeper)
                {
                    diagonalDownLeft.Add(f);
                }
            }

            return diagonalDownLeft;
        }
        
        protected virtual List<Field> GetOrderedFieldsDiagonalUpLeft(Base.Field field)
        {
            List<Base.Field> diagonalUpLeft = new ();
            for (int i = field.X - 1, j = field.Y - 1;
                i >= 0 && j >= 0;
                i--, j--)
            {
                Base.Field f = matrix[i, j];
                if (!f.IsForbidden && !f.IsGoalkeeper)
                {
                    diagonalUpLeft.Add(f);
                }
            }

            return diagonalUpLeft;
        }
    }
}
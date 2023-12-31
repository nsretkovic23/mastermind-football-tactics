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
        public List<Field> Fields => fields;
        [SerializeField] protected short shootingZoneRange;
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
                    matrix[i,j].Position = new Vector2Int(i, j);
                    matrix[i,j].OnFieldClicked += FieldClickedHandler;
                }
            }
        }

        public void FieldClickedHandler(Field field)
        {
            OnFieldClicked?.Invoke(field);
        }

        #region Movement
        public abstract List<Field> GetAvailableFieldsToMoveThePlayer(Field selectedField, Manager manager, Player player);
        #endregion

        #region Passing
        public abstract List<Field> GetAvailableFieldsToPass(Field selectedField, Manager manager, Player player);
        protected abstract List<Field> GetAvailableFieldsToPassFromList(List<Base.Field> directionFields, Base.Player player);
        #endregion

        #region Shooting
        public abstract List<Field> GetAvailableFieldsToShoot(Base.Field selectedField, Base.Manager manager, Base.Player player);
        protected abstract List<Field> GetAvailableFieldsToShootFromList(List<Base.Field> directionFields, Base.Player player);
        protected virtual bool IsInShootingZone(Base.Field selectedField, AttackingDirection attackingDirection)
        {
            // RULE: Selected field is in Shooting zone if it's in the last x columns of the opponent's side, where x is shootingZoneRange variable

            // -+1 because we are not counting goal fields
            if (attackingDirection == AttackingDirection.Right)
            {
                return selectedField.Y >= columns - shootingZoneRange - 1;
            }
            else
            {
                return selectedField.Y < shootingZoneRange + 1;
            }
        }
        #endregion
        
        #region Getting Matrix Fields
        protected virtual List<Field> GetOrderedFieldsToTheRight(Base.Field field, bool includeGoal)
        {
            List<Base.Field> right = new ();
            for (int i = field.Y + 1; i < columns ; i++)
            {
                Base.Field f = matrix[field.X, i];
                if (!f.IsForbidden && (!f.IsGoal || includeGoal))
                {
                    right.Add(f);
                }
            }

            return right;
        }
        
        protected virtual List<Field> GetOrderedFieldsToTheLeft(Base.Field field, bool includeGoal)
        {
            List<Base.Field> left = new ();
            for (int i = field.Y - 1; i >= 0; i--)
            {
                Base.Field f = matrix[field.X, i];
                if (!f.IsForbidden && (!f.IsGoal || includeGoal))
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
        
        protected virtual List<Field> GetOrderedFieldsDiagonalDownRight(Base.Field field, bool includeGoal)
        {
            List<Base.Field> diagonalDownRight = new ();
            for (int i = field.X + 1, j = field.Y + 1;
                i < rows && j < columns;
                i++, j++)
            {
                Base.Field f = matrix[i, j];
                if (!f.IsForbidden && (!f.IsGoal || includeGoal))
                {
                    diagonalDownRight.Add(f);
                }
            }

            return diagonalDownRight;
        }
        
        protected virtual List<Field> GetOrderedFieldsDiagonalUpRight(Base.Field field, bool includeGoal)
        {
            List<Base.Field> diagonalUpRight = new ();
            for (int i = field.X - 1, j = field.Y + 1;
                i >= 0 && j < columns;
                i--, j++)
            {
                Base.Field f = matrix[i, j];
                if (!f.IsForbidden && (!f.IsGoal || includeGoal))
                {
                    diagonalUpRight.Add(f);
                }
            }

            return diagonalUpRight;
        }
        
        protected virtual List<Field> GetOrderedFieldsDiagonalDownLeft(Base.Field field, bool includeGoal)
        {
            List<Base.Field> diagonalDownLeft = new ();
            for (int i = field.X + 1, j = field.Y - 1;
                i < rows && j >= 0;
                i++, j--)
            {
                Base.Field f = matrix[i, j];
                if (!f.IsForbidden && (!f.IsGoal || includeGoal))
                {
                    diagonalDownLeft.Add(f);
                }
            }

            return diagonalDownLeft;
        }
        
        protected virtual List<Field> GetOrderedFieldsDiagonalUpLeft(Base.Field field, bool includeGoal)
        {
            List<Base.Field> diagonalUpLeft = new ();
            for (int i = field.X - 1, j = field.Y - 1;
                i >= 0 && j >= 0;
                i--, j--)
            {
                Base.Field f = matrix[i, j];
                if (!f.IsForbidden && (!f.IsGoal || includeGoal))
                {
                    diagonalUpLeft.Add(f);
                }
            }

            return diagonalUpLeft;
        }
        
        #endregion
    }
}
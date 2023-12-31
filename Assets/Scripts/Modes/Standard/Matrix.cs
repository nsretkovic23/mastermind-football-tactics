using System;
using System.Collections;
using System.Collections.Generic;
using Base;
using UnityEngine;

namespace Modes.Standard
{
    public class Matrix : Base.Matrix
    {
       
        public override List<Base.Field> GetAvailableFieldsToMoveThePlayer(Base.Field selectedField, Base.Manager manager, Base.Player player)
        {
            // TODO: REFACTOR: Refactor these function so that you can obtain fields from left/right/up/down/diagonal and then perform checks for movement passing and shooting, will be cleaner
            
            // TODO: Add additional logic for zones, for example you can't move to column where GK is, or you can't have more than 4/5 players in defense
            // TODO: Extract left/right movement to a separate horizontal movement method, and up/down movement to a separate vertical movement method
            // TODO: Handle GK movement - add only fields from one column (vertical), make this function similar to the one for passing the ball, and then check if field is gk's

            // Add empty fields all the way, if we encounter a field that we can't move to, we don't add it and break the loop 
            // Also break the loop when you encounter a field where the opponent's player is and you can move to, but remember to add that field cause it's still valid
            
            List<Base.Field> availableFields = new List<Base.Field>();

            // Up
            // Here we can move all the way to the top and bottom row, so we can use rows-1
            if (selectedField.Position.x > 0)
            {
                for(int i = selectedField.Position.x - 1; i >= 0; i--)
                {
                    Base.Field field = matrix[i, selectedField.Position.y];
                    if (!field.CanMoveToTheField(player))
                    {
                        break;
                    }

                    availableFields.Add(field);
                    if (!field.IsEmpty)
                    {
                        break;
                    }
                }
            }
            
            // Down
            // Here we can move all the way down to the (rows - 1)th row
            if (selectedField.Position.x < rows - 1)
            {
                for(int i = selectedField.Position.x + 1; i <= rows - 1; i++)
                {
                    Base.Field field = matrix[i, selectedField.Position.y];
                    if (!field.CanMoveToTheField(player))
                    {
                        break;
                    }

                    availableFields.Add(field);
                    if (!field.IsEmpty)
                    {
                        break;
                    }
                }
            }

            if(selectedField.IsGoal)
            {
                return availableFields;
            }
            // Left
            // Compare y with 2 because we can't move to the goalkeeper's column (goalkeeper on the left side, in 0th column), and also we can't move left if we are in the 1st column anyways
            if (selectedField.Position.y > 2)
            {
                for (int i = selectedField.Position.y - 1; i >= 2; i--)
                {
                    Base.Field field = matrix[selectedField.Position.x, i];
                    if (!field.CanMoveToTheField(player))
                    {
                        break;
                    }

                    availableFields.Add(field);
                    if (!field.IsEmpty)
                    {
                        break;
                    }
                }
            }
            
            // Right
            // columns - 2 because we can't move to the goalkeeper's column (goalkeeper on the right side, in (columns - 1) column)
            if (selectedField.Position.y < columns - 2)
            {
                for (int i = selectedField.Position.y + 1; i <= columns - 2; i++)
                {
                    Base.Field field = matrix[selectedField.Position.x, i];
                    if (!field.CanMoveToTheField(player))
                    {
                        break;
                    }

                    availableFields.Add(field);
                    if (!field.IsEmpty)
                    {
                        break;
                    }
                }
            }
            
            // Diagonal up left
            // Minimum x is 1 because need to be below the 0th row to be able to move up-left
            // Minimum y is 2 because the most left row  
            if (selectedField.X > 0 && selectedField.Y > 1)
            {
                for (int i = selectedField.X - 1, j = selectedField.Y - 1;
                     i >= 0 && j >= 1; 
                     --i, --j
                     )
                {
                    Base.Field field = matrix[i, j];
                    if (!field.CanMoveToTheField(player))
                    {
                        break;
                    }

                    availableFields.Add(field);
                    if (!field.IsEmpty)
                    {
                        break;
                    }
                }
            }
            
            // Diagonal down right
            if(selectedField.X < rows-1 && selectedField.Y < columns-1)
            {
                for (int i = selectedField.X + 1, j = selectedField.Y + 1;
                     i < rows && j < columns - 1;
                     i++, j++)
                {
                    Base.Field field = matrix[i, j];
                    if (!field.CanMoveToTheField(player))
                    {
                        break;
                    }

                    availableFields.Add(field);
                    if (!field.IsEmpty)
                    {
                        break;
                    }
                }
            }
            
            // Diagonal up right
            if (selectedField.X > 0 && selectedField.Y < columns - 1)
            {
                for (int i = selectedField.X - 1, j = selectedField.Y + 1;
                    i >= 0 && j < columns - 1;
                    i--, j++)
                {
                    Base.Field field = matrix[i, j];
                    if (!field.CanMoveToTheField(player))
                    {
                        break;
                    }

                    availableFields.Add(field);
                    if (!field.IsEmpty)
                    {
                        break;
                    }
                }
            }
            
            // Diagonal down left
            if (selectedField.X < rows - 1 && selectedField.Y > 1)
            {
                for (int i = selectedField.X + 1, j = selectedField.Y - 1;
                    i < rows && j >= 1;
                    i++, j--)
                {
                    Base.Field field = matrix[i, j];
                    if (!field.CanMoveToTheField(player))
                    {
                        break;
                    }

                    availableFields.Add(field);
                    if (!field.IsEmpty)
                    {
                        break;
                    }
                }
            }
            return availableFields;
        }
        
        public override List<Base.Field> GetAvailableFieldsToPass(Base.Field selectedField, Base.Manager manager, Base.Player player)
        {
            // TODO: Handle long pass zones - these will probably be the ones on the sideline, and you can pass to your teammate across the same column
            // TODO: Otherwise, rules are: you can pass up/down/left/right/diagonally to the closest player, but if player is marked, you can't pass to him
            // TODO: RECONSIDER - you can pass to empty field, for now you can't
            List<Base.Field> availableFields = new List<Base.Field>();
            
            // Left
            List<Base.Field> left = GetOrderedFieldsToTheLeft(selectedField, false);
            availableFields.AddRange(GetAvailableFieldsToPassFromList(left, player));
            
            // Right
            List<Base.Field> right = GetOrderedFieldsToTheRight(selectedField, false);
            availableFields.AddRange(GetAvailableFieldsToPassFromList(right, player));

            // Up
            List<Base.Field> up = GetOrderedFieldsUpwards(selectedField);
            availableFields.AddRange(GetAvailableFieldsToPassFromList(up, player));
            
            // Down
            List<Base.Field> down = GetOrderedFieldsDownwards(selectedField);
            availableFields.AddRange(GetAvailableFieldsToPassFromList(down, player));
            
            // Diagonal up left
            List<Base.Field> diagonalUpLeft = GetOrderedFieldsDiagonalUpLeft(selectedField, false);
            availableFields.AddRange(GetAvailableFieldsToPassFromList(diagonalUpLeft, player));
            
            // Diagonal down left
            List<Base.Field> diagonalDownLeft = GetOrderedFieldsDiagonalDownLeft(selectedField, false);
            availableFields.AddRange(GetAvailableFieldsToPassFromList(diagonalDownLeft, player));
            
            // Diagonal down right
            List<Base.Field> diagonalDownRight = GetOrderedFieldsDiagonalDownRight(selectedField, false);
            availableFields.AddRange(GetAvailableFieldsToPassFromList(diagonalDownRight, player));
            
            // Diagonal up right
            List<Base.Field> diagonalUpRight = GetOrderedFieldsDiagonalUpRight(selectedField, false);
            availableFields.AddRange(GetAvailableFieldsToPassFromList(diagonalUpRight, player));

            return availableFields;
        }
        
        public override List<Base.Field> GetAvailableFieldsToShoot(Base.Field selectedField, Base.Manager manager, Base.Player player)
        {
            // RULE: Shoot can be done only if selectedField is in the shooting zone
            // RULE: Shoot can go horizontally or diagonally, and can only target fields that are marked as IsGoal

            List<Base.Field> availableFields = new();

            if(manager.AttDirection == AttackingDirection.Right && IsInShootingZone(selectedField, AttackingDirection.Right))
            {
                // Right
                List<Base.Field> right = GetOrderedFieldsToTheRight(selectedField, true);
                availableFields.AddRange(GetAvailableFieldsToShootFromList(right, player));
                
                // Diagonal up right
                List<Base.Field> diagonalUpRight = GetOrderedFieldsDiagonalUpRight(selectedField, true);
                availableFields.AddRange(GetAvailableFieldsToShootFromList(diagonalUpRight, player));
                 
                // Diagonal down right
                List<Base.Field> diagonalDownRight = GetOrderedFieldsDiagonalDownRight(selectedField, true);
                availableFields.AddRange(GetAvailableFieldsToShootFromList(diagonalDownRight, player));
            }
            else if (manager.AttDirection == AttackingDirection.Left && IsInShootingZone(selectedField, AttackingDirection.Left))
            {
                // Left
                List<Base.Field> left = GetOrderedFieldsToTheLeft(selectedField, true);
                availableFields.AddRange(GetAvailableFieldsToShootFromList(left, player));
                
                // Diagonal up left
                List<Base.Field> diagonalUpLeft = GetOrderedFieldsDiagonalUpLeft(selectedField, true);
                availableFields.AddRange(GetAvailableFieldsToShootFromList(diagonalUpLeft, player));
                
                // Diagonal down left
                List<Base.Field> diagonalDownLeft = GetOrderedFieldsDiagonalDownLeft(selectedField, true);
                availableFields.AddRange(GetAvailableFieldsToShootFromList(diagonalDownLeft, player));
            }

            return availableFields;
        }
        
        protected override List<Base.Field> GetAvailableFieldsToPassFromList(List<Base.Field> directionFields, Base.Player player)
        {
            List<Base.Field> availableFields = new ();
            foreach (Base.Field field in directionFields)
            {
                if (field.IsEmpty && !field.IsForbidden)
                {
                    continue;
                }

                if (!field.CanPassTheBallToTheField(player))
                {
                    break;
                }
                
                availableFields.Add(field);
                break;
            }

            return availableFields;
        }
        
        protected override List<Base.Field> GetAvailableFieldsToShootFromList(List<Base.Field> directionFields, Player player)
        {
            List<Base.Field> availableFields = new();
            
            directionFields.ForEach(field =>
            {
                if (field.IsGoal && field.IsEmpty)
                {
                    availableFields.Add(field);
                }
            });

            return availableFields;
        }
    }
}
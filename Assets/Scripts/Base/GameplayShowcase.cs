using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Modes.Standard;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

// ReSharper disable All

namespace Base
{
    public enum Action
    {
        Move,
        Pass,
        Shoot,
        None
    }
    public class GameplayShowcase : MonoBehaviour
    {
        [SerializeField] private Manager m1;
        [SerializeField] private Manager m2;
        [SerializeField] private Matrix matrixController;
        [SerializeField] private Ball ball;
        [SerializeField] private TextMeshProUGUI currentlyPlaysText;
        
        private Field firstSelectedField;
        private Field secondSelectedField;

        [field: SerializeField] public Action SelectedAction { get; set; } = Action.None;
        [field: SerializeField] private Field FieldWhereTheBallIs { get; set; }
        
        private Manager currentlyPlays;
        public Manager CurrentlyPlays
        {
            get => currentlyPlays;
            set
            {
                currentlyPlays = value;
                currentlyPlaysText.text = $"Plays: "+ currentlyPlays.Name;
            }
        }
        
        private void Awake()
        {
            matrixController.InitializeMatrix();
            matrixController.OnFieldClicked += FieldClickedHandler;
            PutPlayersOnField();
            
            
            // TODO: Extract this
            CurrentlyPlays = m1;
            SetFieldColliderState(m1.GetFieldsWithPlayers(), true);
            SetFieldColliderState(m2.GetFieldsWithPlayers(), false);
            
            ball.transform.position = 
                CurrentlyPlays == m1 ? 
                    FieldWhereTheBallIs.RightBallHolder.position 
                    : FieldWhereTheBallIs.LeftBallHolder.position;
            
            FieldWhereTheBallIs.Ball = ball;
        }
        
        private void PutPlayersOnField()
        {
            foreach (Player p in  m1.Players)
            {
                p.CurrentField.MoveToTheField(p);
            }
            
            foreach (Player p in  m2.Players)
            {
                p.CurrentField.MoveToTheField(p);
            }
        }

        /// <summary>
        /// Field invokes this method when it's clicked
        /// </summary>
        /// <param name="field">Field that is clicked</param>
        private void FieldClickedHandler(Field field)
        {
            // ==== UNSELECT FIELD ====
            if (firstSelectedField == field || secondSelectedField == field)
            {
                if (firstSelectedField == field)
                {
                    ReturnFieldsToNormalColor();
                    firstSelectedField = null;
                }
                else
                {
                    ReturnFieldsToNormalColor();
                    secondSelectedField = null;
                }

                SelectedAction = Action.None;
                StartCoroutine(ColorLerpCoroutine(field, Color.grey, Color.white, .5f));
                return;
            }
            // ==== END UNSELECT FIELD ====
            
            // TODO: This is instant field selecting, add a delay later
            if(firstSelectedField == null)
            {
                TryToSelectFirstField(field, CurrentlyPlays);
            }
            else
            {
                if (SelectedAction == Action.None)
                {
                    Debug.LogError("Select the action first before selecting the second field");
                    return;
                }
                else
                {
                    TryToSelectSecondField(field);
                    // Action is chosen, and after it second field is clicked, the action should be executed
                    ExecuteAction(SelectedAction);
                    UnselectBothFields();
                    NextTurn(); // TODO: Rename to SwitchTurns()
                }
            }
        }

        private void TryToSelectFirstField(Field field, Manager manager)
        {
            Debug.Log("Trying to select FIRST field: " + field.name);
            
            Player player = field.Players.Find(p => manager.Players.Contains(p));
            if (!player)
            {
                Debug.LogError("This field is either empty or it doesn't contain your player");
                return;
            }

            firstSelectedField = field;

            StartCoroutine(ColorLerpCoroutine(field, Color.white, Color.grey, .5f));
        }
        
        private void TryToSelectSecondField(Field field)
        {
            secondSelectedField = field;
            StartCoroutine(ColorLerpCoroutine(field, Color.white, Color.grey, .5f));
        }
        
        /// <summary>
        /// Invokes when action is pressed (UI button)
        /// Action can be selected only after first field is selected
        /// When action is selected, it enables colliders on available fields that are candidates to be selected as second field
        /// </summary>
        /// <param name="action"></param>
        public void SelectAction(int action)
        {
            if (firstSelectedField == null)
            {
                Debug.LogError("You need to select the field first, before selecting the action");
                return;
            }
            SetFieldColliderState(matrixController.Fields, false);
            
            SelectedAction = (Action)action;
            
            // TODO: See if you can do some abstraction over actions, since they are very similar and have a lot of duplicate code
            if (SelectedAction == Action.Move)
            {
                // Find manager's player that is currently playing on the field where he clicked, since there will be checks when selecting first field
                // This find is completely safe and won't cause null exception
                Player player = firstSelectedField.Players.Find(p => CurrentlyPlays.Players.Contains(p));
                List<Field> availableFields = matrixController.GetAvailableFieldsToMoveThePlayer(firstSelectedField, CurrentlyPlays, player, SelectedAction);
                
                availableFields.ForEach(f => StartCoroutine(ColorLerpCoroutine(f, Color.white, Color.yellow, .5f)));
                // Adding first selected field to its collider can still be enabled, in case you want to deselect the field
                availableFields.AddRange(new List<Field> {firstSelectedField});
                SetFieldColliderState(availableFields, true);
            }
            else if (SelectedAction == Action.Pass)
            {
                if (firstSelectedField.Ball != ball)
                {
                    Debug.LogError("You can't pass the ball from the field that doesn't have the ball");
                    SelectedAction = Action.None;
                    return;
                }
                
                Player player = firstSelectedField.Players.Find(p => CurrentlyPlays.Players.Contains(p));
                List<Field> availableFields = matrixController.GetAvailableFieldsToPass(firstSelectedField, CurrentlyPlays, player, SelectedAction);
                
                availableFields.ForEach(f => StartCoroutine(ColorLerpCoroutine(f, Color.white, Color.cyan, .5f)));
                // Adding first selected field to its collider can still be enabled, in case you want to deselect the field
                availableFields.AddRange(new List<Field> {firstSelectedField});
                SetFieldColliderState(availableFields, true);
            }
        }

        private void SetFieldColliderState(List<Field> fields, bool state)
        {
            fields.ForEach(f => f.Collider.enabled = state);
        }
        
        private void UnselectBothFields()
        {
            // TODO: Don't call ReturnFieldsToNormalColor() as this is called in different context
            if (firstSelectedField != null)
            {
                StartCoroutine(ColorLerpCoroutine(firstSelectedField, Color.grey, Color.white, .5f));
                firstSelectedField = null;
            }
            
            if (secondSelectedField != null)
            {
                StartCoroutine(ColorLerpCoroutine(secondSelectedField, Color.grey, Color.white, .5f));
                secondSelectedField = null;
            }
            
            SelectedAction = Action.None;
        }
        
        private void ReturnFieldsToNormalColor()
        {
            matrixController.Fields.ForEach(f => f.Sprite.color = Color.white);
        }
        
        #region Action Execution
        public void ExecuteAction(Action action)
        {
            if (action == Action.Pass)
            {
                ExecutePass();
            }
            else if (action == Action.Move)
            {
                ExecuteMove();
            }
            // else if (action == Action.Shoot)
            // {
            //     ExecuteShoot();
            // }
        }

        public void ExecuteMove()
        {
            // TODO: Plan for TurnController, where this player finding will be extracted there
            Player player = firstSelectedField.Players.Find(p => CurrentlyPlays.Players.Contains(p));
            StartCoroutine(MoveToNextField(player, firstSelectedField, secondSelectedField));
            
            firstSelectedField.LeaveTheField(player);
            secondSelectedField.MoveToTheField(player);
            player.CurrentField = secondSelectedField;
        }
        
        public void ExecutePass()
        {
            StartCoroutine(PassTheBallCoroutine(firstSelectedField, secondSelectedField));
            FieldWhereTheBallIs = secondSelectedField;
            FieldWhereTheBallIs.Ball = ball;
            firstSelectedField.Ball = null;
        }
        #endregion
        
        # region UI helpers
        public void ReloadScene()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
        
        public void NextTurn()
        {
            ReturnFieldsToNormalColor();
            if (CurrentlyPlays == m1)
            {
                SetFieldColliderState(m2.GetFieldsWithPlayers(), true);
                SetFieldColliderState(m1.GetFieldsWithPlayers(), false);
                CurrentlyPlays = m2;
            }
            else
            {
                SetFieldColliderState(m1.GetFieldsWithPlayers(), true);
                SetFieldColliderState(m2.GetFieldsWithPlayers(), false);
                CurrentlyPlays = m1;
            }
        }
        #endregion

        #region Coroutine Animations

        private IEnumerator MoveToNextField(Player p, Field current, Field next)
        {
            float speed = 5f;
            while (Vector2.Distance(p.transform.position, next.transform.position) > 0.01f)
            {
                p.transform.position = Vector2.MoveTowards(p.transform.position, next.transform.position, speed * Time.deltaTime);
                yield return null;
            }
            p.transform.position = next.transform.position;
        }
        
        /// <summary>
        /// Here we pass the source and destination field and move the ball using MoveTowards
        /// The ball should upscale in the middle of the path and downscale back to normal size when it reaches the destination
        /// </summary>
        private IEnumerator PassTheBallCoroutine(Field source, Field destination)
        {
            Vector3 startingScale = ball.transform.localScale;
            Vector3 sourcePos = source.transform.position;
            Vector3 destPos = CurrentlyPlays == m1 ? destination.RightBallHolder.position : destination.LeftBallHolder.position;
            Vector3 midPoint = (sourcePos + destPos) / 2;

            const float speed = 2f; // Adjust as needed
            const float maxScale = 1.2f; // Adjust as needed

            while (ball.transform.position != destPos)
            {
                // Move the ball towards the destination
                ball.transform.position = Vector3.MoveTowards(ball.transform.position, destPos, speed * Time.deltaTime);

                // Calculate the distance to the midpoint and use it to scale the ball
                float distanceToMidPoint = Vector3.Distance(ball.transform.position, midPoint);
                float scale = maxScale * (1 - distanceToMidPoint / Vector3.Distance(sourcePos, midPoint));
                
                // Prevent scale down below the starting scale
                // ==== Constraint ==== the ball should have the same scale on x and y axis
                float scaleClamp = Mathf.Clamp(scale, startingScale.x, scale); 
                
                ball.transform.localScale = new Vector3(scaleClamp, scaleClamp, scaleClamp);

                yield return null;
            }

            // Set the ball's scale back to normal
            ball.transform.localScale = startingScale;
        }
        
        private IEnumerator ColorLerpCoroutine(Field field, Color start, Color end, float duration)
        {
            float time = 0;
            while (time < duration)
            {
                field.Sprite.color = Color.Lerp(start, end, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            field.Sprite.color = end;
        }
        #endregion
    }
}
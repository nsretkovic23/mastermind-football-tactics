using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Modes.Standard;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

// ReSharper disable All

namespace Base
{
    // TODO: Consider converting it to a class
    public enum Action
    {
        Move,
        Pass,
        Shoot
    }
    public class GameplayShowcase : MonoBehaviour
    {
        [SerializeField] private Manager m1;
        [SerializeField] private Manager m2;
        [SerializeField] private Matrix matrixController;
        [SerializeField] private Ball ball;

        private Field firstSelectedField;
        private Field secondSelectedField;
        
        [field:SerializeField] public Manager CurrentlyPlays { get; set; } 
        [field:SerializeField] public Action SelectedAction { get; set; }


        private void Awake()
        {
            matrixController.InitializeMatrix();
            matrixController.OnFieldClicked += FieldClickedHandler;
            PutPlayersOnField();
            ball.transform.position = ball.CurrentField.LeftBallHolder.position;
            CurrentlyPlays = m1;
        }

        public void ReloadScene()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
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

        private void FieldClickedHandler(Field field)
        {
            // TODO: Remove DRY
            if (firstSelectedField == field)
            {
                firstSelectedField = null;
                Debug.Log("Unselect field animation here");
                StartCoroutine(ColorLerpCoroutine(field, Color.grey, Color.white, .5f));
                
                return;
            }
            // TODO: Remove DRY
            if (secondSelectedField == field)
            {
                secondSelectedField = null;
                Debug.Log("Unselect field animation here");
                StartCoroutine(ColorLerpCoroutine(field, Color.grey, Color.white, .5f));
                
                return;
            }
            
            if(firstSelectedField && secondSelectedField)
            {
                Debug.Log("You already selected two fields, now select the action!");
                return;
            }
            
            TryToSelectFirstField(field, CurrentlyPlays == m1 ? m1 : m2);
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
        
        /// <summary>
        /// UI Buttons
        /// </summary>
        /// <param name="action"></param>
        public void SelectAction(int action)
        {
            SelectedAction = (Action)action;
            
            if (SelectedAction == Action.Move)
            {
                // Find manager's player that is currently playing on the field where he clicked, since there will be checks when selecting first field
                // This find is completely safe and won't cause null exception
                Player player = firstSelectedField.Players.Find(p => CurrentlyPlays.Players.Contains(p));
                List<Field> availableFields = matrixController.GetAvailableFieldsToMoveThePlayer(firstSelectedField, CurrentlyPlays, player, SelectedAction);
                
                availableFields.ForEach(f => StartCoroutine(ColorLerpCoroutine(f, Color.white, Color.yellow, .5f)));
            }
            else if (SelectedAction == Action.Pass)
            {
                Player player = firstSelectedField.Players.Find(p => CurrentlyPlays.Players.Contains(p));
                List<Field> availableFields = matrixController.GetAvailableFieldsToPass(firstSelectedField, CurrentlyPlays, player, SelectedAction);
                
                availableFields.ForEach(f => StartCoroutine(ColorLerpCoroutine(f, Color.white, Color.cyan, .5f)));
            }
        }
        
        // TODO: Consider refactoring it, when possible fields for certain actions are obtained, just enable their colliders
        private void TryToSelectSecondField(Field field, Manager manager)
        {
            Debug.Log("Trying to select SECOND field: " + field.name);

            // HANDLE MOVE ACTION
            if(SelectedAction == Action.Move)
            {
                if (field.Players.Count == 0)
                {
                    // No players on the field, you can move
                    // TODO: Coroutine for moving the player from field to field
                    // TODO: There might be a ball on this empty field, handle this case
                }
                else if (field.Players.Count == 1 && !manager.Players.Contains(field.Players[0]) && (ball.CurrentField != field && field.Ball == null))
                {
                    // On the field is opponent player, you can move and mark him, only if he doesn't have the ball
                    // TODO: Coroutine for moving the player from field to field
                }
                else
                {
                    Debug.LogError("You can't move to this field");
                }
            }

            // HANDLE PASS ACTION
            if (SelectedAction == Action.Pass)
            {
                
            }
        }

        private void TryToSelectField(Field field, Manager manager)
        {
            Debug.Log("Trying to select field: " + field.name);

            if (field.Players.Count == 0)
            {
                // TODO: This also prevent passing the ball to the empty field, think about it if it should be allowed
                Debug.LogError("You can't click on empty field, select field where your player is");
                return;
            }

            Player player = field.Players.Find(p => manager.Players.Contains(p));
            if (!player)
            {
                Debug.LogError("You can't click on field where the opponent player is first. Select your player and then you can maybe move to this field to mark the player");
                return;
            }

            if (!firstSelectedField)
            {
                firstSelectedField = field;

                StartCoroutine(ColorLerpCoroutine(field, Color.white, Color.grey, .5f));
                return;
            }
            
            if (!secondSelectedField)
            {
                secondSelectedField = field;
                Debug.Log("You selected two fields, now select the action!");
                StartCoroutine(ColorLerpCoroutine(field, Color.white, Color.grey, .5f));
                return;
            }
        }
        
        public void ExecutePass()
        {
            if (!firstSelectedField || !secondSelectedField)
            {
                Debug.LogError("You need to select source and destination field where you want to pass the ball first");
                return;
            }

            if (firstSelectedField.Ball == null)
            {
                Debug.LogError("First selected field doesn't have the ball, you can't pass the ball from this field");
                return;
            }

            StartCoroutine(ColorLerpCoroutine(firstSelectedField, Color.grey, Color.white, .5f));
            StartCoroutine(ColorLerpCoroutine(secondSelectedField, Color.grey, Color.white, .5f));
            
            StartCoroutine(PassTheBallCoroutine(firstSelectedField, secondSelectedField));
        }

        #region Coroutine Animations
        
        /// <summary>
        /// Here we pass the source and destination field and move the ball using MoveTowards
        /// The ball should upscale in the middle of the path and downscale back to normal size when it reaches the destination
        /// </summary>
        private IEnumerator PassTheBallCoroutine(Field source, Field destination)
        {
            Vector3 startingScale = ball.transform.localScale;
            Vector3 sourcePos = source.transform.position;
            Vector3 destPos = destination.transform.position;
            Vector3 midPoint = (sourcePos + destPos) / 2;

            const float speed = 2f; // Adjust as needed
            const float maxScale = 1.5f; // Adjust as needed

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
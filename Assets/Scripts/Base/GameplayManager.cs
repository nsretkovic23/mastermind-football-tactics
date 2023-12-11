using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ReSharper disable CommentTypo

namespace Base
{
    public abstract class GameplayManager : MonoBehaviour
    {
        [SerializeField] protected Matrix matrixController;
        // ::::::::Rules::::::::
        // ==== MOVEMENT =====
        // You can stack up to two players (player and opponent) on a field
        // In one move you can make two decisions: Move with or without the ball, pass or shoot
        // You can move straight or diagonal aswell, but, when you encounter an opponent, you can't move further, but you can stack your player on the same field as the opponent
        // You can have only up to 4 players in defensive zone, you can move all of them into attack or midfield if you want
        // You have one goalkeeper, and you can't move him out of the goal, goalkeeper has 3 fields in a column where he can move
        // ==== PASSING =====
        // Passes and shots can be straight and diagonal
        // You CAN'T pass to a field that is occupied by both player and opponent
        // You CAN pass to opponent, that way when you are surounded you can pass to the opponent and play defense
        // Passes go until the first player
        // There are no passes to the goalkeeper
        // If you have a player on a field by the sideline, and if that player has the ball there, you can switch the play across whole matrix column, and choose any free player
        // You should check for an edge case where you are surrounded by two-player stack, and you can't pass to any of them, you should maybe show a message that possesion is lost, and another player should receive the ball in defence
        // ==== SHOOTING =====
        // Shooting area is the last 3 columns of the matrix, that includes Attacking zone and the closest column of the midfield to the goal
        // You can score the goal only if all fields on the way to the goal are free being it straight or diagonal

        protected virtual void Awake()
        {
            Debug.Log("Hello from base GameplayManager");
            //matrixController.InitializeMatrix();
        }
    }
}
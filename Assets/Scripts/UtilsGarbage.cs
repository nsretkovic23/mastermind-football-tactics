using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilsGarbage : MonoBehaviour
{
    private void CreateBallHoldersInOnValidate()
    {
        // These are fake objects, just so it doesn't throw compile errors
        Transform LeftBallHolder = new RectTransform();
        Transform RightBallHolder = new RectTransform();
        // Create 2 child game objects that will serve as holders for the balls,
        // since field is a square, first holder should be on local position of .5f on x, and -.15 on y
        // second holder should be on local position of -.5f on x, and -.15 on y
        // Store them into LeftBallHolder and RightBallHolder, first object should be LeftBallHolder, second RightBallHolder
        // Check if the holders are not already assigned
        
        if (LeftBallHolder == null)
        {
            bool contains = false;
            foreach (Transform child in transform)
            {
                if (child.name == "LeftBallHolder")
                {
                    contains = true;
                    LeftBallHolder = child;
                    break;
                }
            }

            if (!contains)
            {
                // Create a new game object for the left ball holder
                GameObject leftBallHolderObject = new GameObject("LeftBallHolder");
                leftBallHolderObject.transform.SetParent(transform, false);
                leftBallHolderObject.transform.localPosition = new Vector3(-.5f, -.15f, 0);
                LeftBallHolder = leftBallHolderObject.transform;
            }
        }
        
        if (RightBallHolder == null)
        {
            bool contains = false;
            foreach (Transform child in transform)
            {
                if (child.name == "RightBallHolder")
                {
                    contains = true;
                    break;
                }
            }
            if (!contains)
            {
                // Create a new game object for the left ball holder
                GameObject rightBallHolderObject = new GameObject("RightBallHolder");
                rightBallHolderObject.transform.SetParent(transform, false);
                rightBallHolderObject.transform.localPosition = new Vector3(.5f, -.15f, 0);
                RightBallHolder = rightBallHolderObject.transform;
            }
        }
    }
}

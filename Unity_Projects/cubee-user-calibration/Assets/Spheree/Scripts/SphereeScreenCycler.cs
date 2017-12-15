using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SphereeScreenCycler : MonoBehaviour {

    
    public Transform projectorParent;
    public Text mapText;
    private int stateID;
    public int numStates;
    public Camera[] projectors;
    private List<List<int>> states;

    #region monobehavior
    void Start () {
        stateID = 0;
        projectors = new Camera[projectorParent.childCount];
        for (int i = 0; i < projectorParent.childCount; i++)
        {
            projectors[i] = projectorParent.GetChild(i).gameObject.GetComponent<Camera>();
        }



        int numProjectors = projectors.Length;
        numStates = factorial(numProjectors);

        // Generate a list of target display numbers
        List<int> initialSequence = new List<int>();
        for (int i = 0; i<numProjectors; i++)
        {
            initialSequence.Add(i+1);
        }

        // Calculate all possible permutations of target display numbers
        Sequencer<int> mySequencer = new Sequencer<int>(initialSequence);
        states = mySequencer.sequence();
        numStates = states.Count;
	}
    #endregion

    /// <summary>
    /// Cycles through the possible projector to screen mappings
    /// </summary>
    public void cycleScreens()
    {
        stateID++;
        string mapString = "";
        
        // Assign each projectors target display
        List<int> currentState = states[stateID % numStates];
        for(int i = 0; i < projectors.Length; i++)
        {
            projectors[i].targetDisplay = currentState[i];
            mapString += currentState[i].ToString() + "  ";
        }

        // Display order to text box
        if (mapText != null)
        {
            mapText.text = mapString;
        }
    }

    /// <summary>
    /// This class is designed to be able to generate a list of all possible sequences of an input list 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private class Sequencer<T>
    {
        private List<T> inputPattern;
        public Sequencer(List<T> newInputPattern){
            inputPattern = newInputPattern;
        }
        public List<List<T>> sequence()
        {
            return sequence(inputPattern);
        }
        /// <summary>
        /// Return a list of lists containting all possible prmutations of an input list
        /// </summary>
        /// <param name="pattern"> list of inputs </param>
        /// <returns></returns>
        public List<List<T>> sequence(List<T> pattern)
        {
            int n = pattern.Count;

            // Base case, return the input in a list of lists
            if (n == 1)
            {
                return new List<List<T>> { new List<T> { pattern[0] } };
            }
            List<List<T>> returnList = new List<List<T>>();
            List<List<T>> tempList;

            // Recursive Case, Remove each element from the input list and sequence the rest
            for (int i = 0; i < n; i++)
            {
                T element = pattern[i];
                pattern.RemoveAt(i);
                tempList = sequence(pattern);
                foreach (List<T> sequence in tempList)
                {
                    sequence.Insert(0, element);
                    returnList.Add(sequence);
                }

                pattern.Insert(i, element);
            }
            return returnList;
        }
    }
    
    /// <summary>
    /// Calculate the factorial of an integer number
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    private int factorial(int num)
    {
        if (num <= 0)
        {
            return 1;
        }
        return factorial(num - 1) * num;
    }
}

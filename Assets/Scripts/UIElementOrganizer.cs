using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIElementOrganizer : MonoBehaviour
{
    public List<List<Selectable>> KeyRows;  // List of rows, each containing selectable UI elements (keys).

    private void Awake()
    {
        Reload();
    }
    public void Reload()
    {
        PopulateKeyRows();
        SetInitialSelection();
        //PrintKeyRows();
    }
    public void PrintKeyRows()
    {
        for (int row = 0; row < KeyRows.Count; row++)
        {
            string rowString = "Row " + row + ": ";
            for (int col = 0; col < KeyRows[row].Count; col++)
            {
                Selectable key = KeyRows[row][col];
                rowString += key.gameObject.name + " on " + key.transform.position.x + " x and " + key.transform.position.y + " y;" ;
            }
            Debug.Log(rowString);
        }
    }
    public Selectable GetNextSelectable(Selectable currentKey, Vector2 direction)
    {
        int currentRow = -1;
        int currentCol = -1;

        // Find the current key's position in the 2D list
        for (int i = 0; i < KeyRows.Count; i++)
        {
            if (KeyRows[i].Contains(currentKey))
            {
                currentRow = i;
                currentCol = KeyRows[i].IndexOf(currentKey);
                break;
            }
        }

        if (currentRow == -1 || currentCol == -1)
        {
            Debug.LogError("Current key not found in the key rows.");
            return null;
        }

        int nextRow = currentRow;
        int nextCol = currentCol;

        if (Math.Abs(direction.x) >= Math.Abs(direction.y))
        {
            if (direction.x <= 0)
            {
                nextCol = (currentCol - 1 + KeyRows[currentRow].Count) % KeyRows[currentRow].Count;
            }
            else if (direction.x >= 0)
            {
                nextCol = (currentCol + 1) % KeyRows[currentRow].Count;
            }
        }
        else
        {
            if (direction.y >= 0)
            {
                // Move to the row above and find the closest key by anchoredPosition.x
                nextRow = (currentRow - 1 + KeyRows.Count) % KeyRows.Count;
                Selectable closestKey = FindClosestKeyByX(KeyRows[nextRow], currentKey.GetComponent<RectTransform>().anchoredPosition.x);
                nextCol = KeyRows[nextRow].IndexOf(closestKey);
            }
            else if (direction.y <= 0)
            {
                // Move to the row below and find the closest key by anchoredPosition.x
                nextRow = (currentRow + 1) % KeyRows.Count;
                Selectable closestKey = FindClosestKeyByX(KeyRows[nextRow], currentKey.GetComponent<RectTransform>().anchoredPosition.x);
                nextCol = KeyRows[nextRow].IndexOf(closestKey);
            }
        }

        return KeyRows[nextRow][nextCol];
    }

    // Method to find the closest key by anchoredPosition.x
    private Selectable FindClosestKeyByX(List<Selectable> row, float xPosition)
    {
        Selectable closestKey = null;
        float closestDistance = float.MaxValue;

        foreach (var key in row)
        {
            float distance = Mathf.Abs(key.GetComponent<RectTransform>().anchoredPosition.x - xPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestKey = key;
            }
        }

        return closestKey;
    }
    public void PopulateKeyRows()
    {
        KeyRows = new List<List<Selectable>>();
        // Find all Selectable keys in the scene
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas not found in parent hierarchy.");
            return;
        }

        Selectable[] selectables = canvas.GetComponentsInChildren<Selectable>();

        // Group keys by their y position (rounded to avoid precision issues)
        var groupedKeys = selectables
            .GroupBy(key => Mathf.Round(key.transform.position.y * 1000f) / 1000f)
            .OrderByDescending(group => group.Key);

        // Clear existing rows
        KeyRows.Clear();

        // Add each group of keys as a row
        foreach (var group in groupedKeys)
        {
            List<Selectable> row = group
                .OrderBy(key => key.GetComponent<RectTransform>().anchoredPosition.x)
                .ToList();
            //List<Selectable> row = group
            //    .OrderBy(key => key.transform.GetSiblingIndex())
            //    .ToList();
            KeyRows.Add(row);
        }
    }

    private void SetInitialSelection()
    {
        if (KeyRows.Count > 0 && KeyRows[0].Count > 0)
        {
            // Set the first element of the first row as the initial selection
            SelectedObject = KeyRows[0][0].gameObject;
        }
    }

    public GameObject SelectedObject { get; private set; }
}
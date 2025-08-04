using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
public class GridManager : MonoBehaviour
{
    [HideInInspector]
    public Dictionary<(int x, int y), Company> grid = new();
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    public Dictionary<(int x, int y), Company> GetGrid() => grid;
    public int Width => width;
    public int Height => height;
    public bool AddCompany(Company company)
    {
        if (grid.ContainsKey(company.Position))
            return false; // Tile already occupied

        grid[company.Position] = company;
        return true;
    }
    public bool RemoveCompany(Company company)
    {
        var pos = company.Position;

        // Ensure this company is really the one in that tile
        if (grid.TryGetValue(pos, out var existingCompany))
        {
            if (existingCompany == company)
            {
                grid.Remove(pos);
                return true;
            }
        }
        else Debug.LogError($"Cannot remove company {company.Position}");
        return false; // Company not found at its expected position
    }

    public void RemoveAll()
    {
        grid.Clear();
    }
    
    public bool MoveCompany(Company company, int newX, int newY)
    {
        var currentPos = company.Position;
        var newPos = (newX, newY);

        // Prevent moving into occupied tile
        if (grid.ContainsKey(newPos))
            return false;

        // Remove from old position
        grid.Remove(currentPos);

        // Add to new position
        grid[newPos] = company;

        // Update position on the company
        company.X = newX;
        company.Y = newY;

        return true;
    }
    
    public bool IsOccupied((int x, int y) pos)
    {
        if (!grid.TryGetValue(pos, out var company))
            return false;
        return true;
    }
    
    public List<Company> GetNeighbors(Company company,List<Hero> acceptableHeroes,(int dx, int dy) lookAt=default)
    {
        var dirs = new List<(int dx, int dy)>();
        if (lookAt == default) dirs.AddRange(new[] { (1, 0), (-1, 0), (0, 1), (0, -1) });
        else dirs.Add(lookAt);
        
        var neighbors = new List<Company>();

        foreach (var (dx, dy) in dirs)
        {
            var checkPos = (company.X + dx, company.Y + dy);
            if (grid.TryGetValue(checkPos, out var neighbor))
            {
                if (acceptableHeroes.Contains(neighbor.unitOwner)) neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }
    [CanBeNull]
    public Company GetCompanyAt(int x, int y)
    {
        grid.TryGetValue((x, y), out var company);
        return company;
    }
    public List<Company> GetCompanyAt(List<(int x, int y)> PosList)
    {
        List<Company> newList = new List<Company>();
        foreach (var pos in PosList)
        {
            if (grid.TryGetValue(pos, out var company)) newList.Add(company);
        }
        return newList;
    }
    public (int minX, int maxX, int minY, int maxY) GetGridBounds()
    {
        if (grid.Count == 0)
            return (0, 0, 0, 0); // or throw, or return default

        var xs = grid.Keys.Select(k => k.x);
        var ys = grid.Keys.Select(k => k.y);

        int minX = xs.Min();
        int maxX = xs.Max();
        int minY = ys.Min();
        int maxY = ys.Max();

        return (minX, maxX, minY, maxY);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int maxLives = 5;
    public int currentLives;

    public event Action<int> OnLivesChanged;

    private void Awake()
    {
        if (Instance == null)
        {
           Instance = this;
           DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentLives = maxLives;

        OnLivesChanged?.Invoke(currentLives);
    }

    public void ProcessPlayerDamage(int damage)
    {
        currentLives -= damage;

        if(currentLives < 0) currentLives = 0;

        OnLivesChanged?.Invoke(currentLives);

    }
    
    public void lifeReset()
    {
        currentLives = maxLives;

        OnLivesChanged?.Invoke(currentLives);
    }
}

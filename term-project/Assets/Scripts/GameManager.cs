using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameState { Aim, Fire, Resolve, GameOver }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState State { get; private set; } = GameState.Aim;
    public int CurrentLevel => LevelData.CurrentLevel;
    public int MaxAmmo => LevelData.MaxAmmo;
    public int Ammo { get; private set; }
    public int TotalTargets { get; private set; }
    private int destroyedTargets;

    private CannonController cannon;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(InitGame());
    }

    private IEnumerator InitGame()
    {
        yield return null;

        cannon = FindObjectOfType<CannonController>();
        CannonController.OnFire += OnFireRequested;
        BuildCastle();
        Ammo = MaxAmmo;
        GameUI.Instance.UpdateAmmo(Ammo, MaxAmmo);
    }

    private void OnDestroy()
    {
        CannonController.OnFire -= OnFireRequested;
    }

    private void BuildCastle()
    {
        var castle = CastleBuilder.BuildCastle(LevelData.CastlePosition, LevelData.CurrentLevel);
        TotalTargets = 0;
        foreach (var d in FindObjectsOfType<Destructible>())
        {
            if (d.IsTarget) TotalTargets++;
        }
        destroyedTargets = 0;
    }

    private void OnFireRequested(Vector3 pos, Vector3 dir, float power)
    {
        if (State != GameState.Aim || Ammo <= 0) return;
        Ammo--;
        GameUI.Instance.UpdateAmmo(Ammo, MaxAmmo);
        Projectile.Create(pos, dir, power);
        StartCoroutine(FireSequence());
    }

    private IEnumerator FireSequence()
    {
        State = GameState.Fire;
        
        // Switch to cinematic camera angle during flight
        Vector3 originalCamPos = Camera.main.transform.position;
        Vector3 originalCamLook = new Vector3(0, 5, 15);
        Camera.main.transform.position = new Vector3(15, 8, 95);
        Camera.main.transform.LookAt(new Vector3(0, 5, 40));
        
        yield return null;

        // Wait for all projectiles to settle or be destroyed
        yield return new WaitForSeconds(1f);
        yield return WaitForProjectilesToSettle();

        // Restore original camera
        Camera.main.transform.position = originalCamPos;
        Camera.main.transform.LookAt(originalCamLook);

        State = GameState.Resolve;
        CheckWinLose();
    }

    private void Update()
    {
        // Level select cheat codes (works anytime)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            LoadLevel(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LoadLevel(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            LoadLevel(2);
        }

        // Next level / restart when game over
        if (State == GameState.GameOver)
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                OnNextLevel();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartLevel();
            }
        }
    }

    private void LoadLevel(int level)
    {
        LevelData.CurrentLevel = Mathf.Clamp(level, 0, 2);
        RestartLevel();
    }

    private IEnumerator WaitForProjectilesToSettle()
    {
        float timeout = 8f;
        float elapsed = 0f;

        while (elapsed < timeout)
        {
            bool allSettled = true;
            foreach (var p in FindObjectsOfType<Projectile>())
            {
                if (p.rb.velocity.sqrMagnitude > 0.5f)
                {
                    allSettled = false;
                    break;
                }
            }
            if (allSettled) yield break;
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void CheckWinLose()
    {
        int remaining = TotalTargets - destroyedTargets;
        if (remaining <= 0)
        {
            State = GameState.GameOver;
            GameUI.Instance.ShowWin();
            return;
        }
        if (Ammo <= 0)
        {
            State = GameState.GameOver;
            GameUI.Instance.ShowLose();
            return;
        }
        State = GameState.Aim;
    }

    public void RestartLevel()
    {
        // Destroy existing castle and all destructibles
        var castle = GameObject.Find("Castle");
        if (castle != null) Destroy(castle);

        foreach (var p in FindObjectsOfType<Projectile>())
            Destroy(p.gameObject);

        // Rebuild
        BuildCastle();
        Ammo = MaxAmmo;
        State = GameState.Aim;
        GameUI.Instance.UpdateAmmo(Ammo, MaxAmmo);
        GameUI.Instance.UpdateLevel();
        GameUI.Instance.HideGameOver();
    }

    private void OnNextLevel()
    {
        LevelData.CurrentLevel++;
        if (LevelData.CurrentLevel >= 3)
        {
            GameUI.Instance.ShowWin();
            return;
        }
        RestartLevel();
    }

    public void TargetDestroyed()
    {
        destroyedTargets++;
        CheckWinLose();
    }
}

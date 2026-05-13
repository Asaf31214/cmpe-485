using UnityEngine;
using System.Collections;

public enum GameState { Aim, Fire, Resolve, GameOver }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private Texture2D _blockTexture;
    private Texture2D _anchorTexture;

    public GameState State { get; private set; } = GameState.Aim;
    public int Ammo { get; private set; }
    public int TargetsRemaining { get; private set; }

    private CannonController cannon;

    private void Awake()
    {
        Instance = this;
    }

    public void SetTextures(Texture2D blockTexture, Texture2D anchorTexture)
    {
        _blockTexture = blockTexture;
        _anchorTexture = anchorTexture;
    }

    private void Start()
    {
        cannon = FindObjectOfType<CannonController>();
        CannonController.OnFire += HandleFire;
        Destructible.OnTargetDestroyed += HandleTargetDestroyed;
        LoadLevel(LevelData.CurrentLevel);
    }

    private void OnDestroy()
    {
        CannonController.OnFire -= HandleFire;
        Destructible.OnTargetDestroyed -= HandleTargetDestroyed;
    }

    private void Update()
    {
        HandleLevelSelect();
        HandleGameOverInput();
    }

    private void HandleLevelSelect()
    {
        for (int i = 0; i < LevelData.TotalLevels; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                LoadLevel(i);
            }
        }
    }

    private void HandleGameOverInput()
    {
        if (State != GameState.GameOver) return;

        if (Input.GetKeyDown(KeyCode.N))
        {
            int next = LevelData.CurrentLevel + 1;
            if (LevelData.IsValidLevel(next))
            {
                LoadLevel(next);
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            LoadLevel(LevelData.CurrentLevel);
        }
    }

    private void LoadLevel(int levelIndex)
    {
        if (!LevelData.IsValidLevel(levelIndex)) return;

        LevelData.CurrentLevel = levelIndex;

        ClearScene();
        BuildCastle();
        ResetAmmo();
        ResetState();

        GameUI.Instance.Refresh();
    }

    private void ClearScene()
    {
        var castle = GameObject.Find("Castle");
        if (castle != null) Destroy(castle);

        foreach (var p in FindObjectsOfType<Projectile>())
            Destroy(p.gameObject);
    }

    private void BuildCastle()
    {
        CastleBuilder.BuildCastle(LevelData.CastlePosition, LevelData.GetCurrentLayout(), _blockTexture, _anchorTexture);
        TargetsRemaining = LevelData.GetTargetCount();
    }

    private void ResetAmmo()
    {
        Ammo = LevelData.MaxAmmo;
    }

    private void ResetState()
    {
        State = GameState.Aim;
        GameUI.Instance.HideGameOver();
    }

    private void HandleFire(Vector3 pos, Vector3 dir, float power)
    {
        if (State != GameState.Aim || Ammo <= 0) return;

        Ammo--;
        GameUI.Instance.UpdateAmmo(Ammo);
        Projectile.Create(pos, dir, power);
        StartCoroutine(FireSequence());
    }

    private void HandleTargetDestroyed()
    {
        TargetsRemaining--;
        GameUI.Instance.UpdateTargets(TargetsRemaining);
        CheckWinLose();
    }

    private IEnumerator FireSequence()
    {
        State = GameState.Fire;

        yield return CinematicCamera();

        yield return WaitForProjectilesToSettle();

        State = GameState.Resolve;
        CheckWinLose();
    }

    private IEnumerator CinematicCamera()
    {
        var cam = Camera.main;
        Vector3 originalPos = cam.transform.position;
        Quaternion originalRot = cam.transform.rotation;

        cam.transform.position = new Vector3(5, 8, 65);
        cam.transform.LookAt(new Vector3(0, 5, 40));

        yield return new WaitForSeconds(1f);
        yield return WaitForProjectilesToSettle();
        yield return new WaitForSeconds(1f);

        cam.transform.position = originalPos;
        cam.transform.rotation = originalRot;
    }

    private IEnumerator WaitForProjectilesToSettle()
    {
        float timeout = 8f;
        float elapsed = 0f;

        while (elapsed < timeout)
        {
            bool settled = true;
            foreach (var p in FindObjectsOfType<Projectile>())
            {
                if (p.Rigidbody.velocity.sqrMagnitude > 0.5f)
                {
                    settled = false;
                    break;
                }
            }
            if (settled) yield break;
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    
    private void CheckWinLose()
    {
        if (TargetsRemaining <= 0)
        {
            State = GameState.GameOver;
            GameUI.Instance.ShowWin(LevelData.CurrentLevel >= LevelData.TotalLevels - 1);
        }
        else if (Ammo <= 0 && State == GameState.Resolve)
        {
            State = GameState.GameOver;
            GameUI.Instance.ShowLose();
        }
        else
        {
            State = GameState.Aim;
        }
    }
}
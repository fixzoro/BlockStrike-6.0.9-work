using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class PlayerSkin : MonoBehaviour
{
    public bool isPlayerActive;

    public byte Health;

    public Team PlayerTeam;

    private bool StartDamage;

    public ControllerManager Controller;

    public PlayerAnimator PlayerAnimator;

    public PlayerSkinRagdoll PlayerRagdoll;

    public Transform[] PlayerLimbs;

    public PlayerSkinAtlas PlayerAtlas;

    public Transform[] PlayerWeaponContainers;

    public Transform PlayerWeaponRoot;

    public Transform PlayerTwoWeaponRoot;

    public AudioClip[] PlayerFoosteps;

    public AudioSource cachedAudioSource;

    public Transform PlayerTransform;

    public Transform PlayerSpectatePoint;

    public GameObject PlayerRoot;

    public PlayerSkinDamage[] PlayerDamages;

    public nTimer Timer;

    public PlayerSounds Sounds;

    private bool Sound = true;

    private bool ShowDamage;

    public float PhotonSpeed = 10f;

    public Vector3 PhotonPosition = Vector3.zero;

    public Quaternion PhotonRotation = Quaternion.identity;

    public bool Dead;

    private float Move;

    public float Rotate;

    private Tweener RotateTween;

    private bool Foostep;

    private bool visible;

    public int HeadSkin = -1;

    public int BodySkin = -1;

    public int LegsSkin = -1;

    private bool activeRagdoll;

    public TPWeaponShooter SelectWeapon;

    private List<TPWeaponShooter> WeaponsList = new List<TPWeaponShooter>();

    public void Start()
    {
        if (this.Controller == null)
        {
            this.Controller = this.PlayerTransform.root.GetComponent<ControllerManager>();
        }
        this.Timer.In(0.15f, true, new TimerDelegate(this.CheckPosition));
        this.Timer.In(0.12f, true, new TimerDelegate(this.UpdateMove));
        EventManager.AddListener("OnSettings", new EventManager.Callback(this.UpdateSettings));
        this.UpdateSettings();
        if (this.RotateTween != null)
        {
            this.RotateTween.Kill(false);
        }
        this.RotateTween = DOTween.To(() => this.Rotate, delegate (float x)
        {
            this.Rotate = x;
        }, 0f, 0.15f).SetAutoKill(false).SetUpdate(UpdateType.Late).OnUpdate(new TweenCallback(this.UpdateRotate)).SetEase(Ease.Linear);
    }

    public void OnDefault()
    {
        this.PlayerRoot.SetActive(false);
        this.PhotonPosition = Vector3.zero;
        this.PhotonRotation = Quaternion.identity;
        this.isPlayerActive = false;
        this.Health = 0;
        this.PlayerTeam = Team.None;
        this.Dead = false;
        this.Rotate = 0f;
        this.HeadSkin = -1;
        this.BodySkin = -1;
        this.LegsSkin = -1;
        this.StartDamage = false;
        this.Sound = true;
        this.ShowDamage = false;
        this.Move = 0f;
        this.Foostep = false;
        this.visible = false;
        if (this.activeRagdoll)
        {
            this.DeactiveRagdoll();
        }
        this.activeRagdoll = false;
        this.Sounds.OnDefault();
        this.PlayerAnimator.OnDefault();
    }

    private void UpdateSettings()
    {
        this.Sound = Settings.Sound;
        this.ShowDamage = Settings.ShowDamage;
    }

    public void OnEnableRoot()
    {
        this.Foostep = false;
        this.isPlayerActive = true;
        this.PlayerAnimator.SetDefault();
        this.RotateTween.Pause<Tweener>();
    }

    public void OnDisableRoot()
    {
        this.isPlayerActive = false;
        this.RotateTween.Play<Tweener>();
    }

    private void CheckPosition()
    {
        nProfiler.BeginSample("CheckPosition");
        if (this.visible && !this.Dead && (this.PlayerTransform.localPosition.x - this.PhotonPosition.x > 3f || this.PlayerTransform.localPosition.y - this.PhotonPosition.y > 3f || this.PlayerTransform.localPosition.z - this.PhotonPosition.z > 3f))
        {
            this.PlayerTransform.localPosition = this.PhotonPosition;
        }
        nProfiler.EndSample();
    }

    private void LateUpdate()
    {
        if (this.visible)
        {
            if (this.PlayerTransform.localPosition != this.PhotonPosition)
            {
                this.PlayerTransform.localPosition = this.Vector3Lerp(this.PlayerTransform.localPosition, this.PhotonPosition, Time.deltaTime * this.PhotonSpeed);
            }
            if (this.PlayerTransform.localRotation != this.PhotonRotation)
            {
                this.PlayerTransform.localRotation = Quaternion.Lerp(this.PlayerTransform.localRotation, this.PhotonRotation, Time.deltaTime * this.PhotonSpeed);
            }
        }
        else
        {
            if (this.PlayerTransform.localPosition != this.PhotonPosition)
            {
                this.PlayerTransform.localPosition = this.PhotonPosition;
            }
            if (this.PlayerTransform.localRotation != this.PhotonRotation)
            {
                this.PlayerTransform.localRotation = this.PhotonRotation;
            }
        }
    }

    private Vector3 Vector3Lerp(Vector3 from, Vector3 to, float t)
    {
        from.x += (to.x - from.x) * t;
        from.y += (to.y - from.y) * t;
        from.z += (to.z - from.z) * t;
        return from;
    }

    private void UpdateMove()
    {
        nProfiler.BeginSample("UpdateMove");
        if (this.isPlayerActive)
        {
            if (this.visible)
            {
                this.PlayerAnimator.move = this.Move;
            }
            if (!this.Foostep && this.Sound && Mathf.Abs(this.Move) >= 0.6f)
            {
                this.UpdateFoosteps();
            }
        }
        nProfiler.EndSample();
    }

    private void UpdateRotate()
    {
        this.PlayerAnimator.rotate = this.Rotate;
    }

    public void SetMove(float move)
    {
        this.Move = move;
    }

    public float GetMove()
    {
        return this.Move;
    }

    public void SetRotate(float rotate)
    {
        if (this.visible && !this.Dead)
        {
            if (this.RotateTween != null)
            {
                this.RotateTween.ChangeStartValue(this.Rotate, -1f);
                this.RotateTween.ChangeEndValue(rotate, -1f, false).Restart(true, -1f);
            }
        }
        else
        {
            this.Rotate = rotate;
            this.UpdateRotate();
        }
    }

    public void SetGrounded(bool grounded)
    {
        if (this.isPlayerActive)
        {
            this.PlayerAnimator.grounded = grounded;
        }
    }

    public void SetPlayerVisible(bool value)
    {
        this.visible = value;
        this.PlayerAnimator.visible = value;
        if (!this.visible)
        {
            this.PlayerAnimator.move = 0f;
            this.RotateTween.Pause<Tweener>();
        }
        else
        {
            this.RotateTween.Play<Tweener>();
        }
    }

    public void SetWeapon(WeaponData weaponType, int skinID, int fireStat, byte[] stickers)
    {
        this.PlayerAnimator.SetWeapon(weaponType.Animation);
        if (this.SelectWeapon != null && this.SelectWeapon.name == weaponType.Name)
        {
            return;
        }
        if (this.SelectWeapon != null)
        {
            this.SelectWeapon.Deactive();
        }
        TPWeaponShooter tpweaponShooter = this.ContainsWeapon(weaponType.Name);
        if (tpweaponShooter == null)
        {
            GameObject gameObject = Utils.AddChild(weaponType.TpsPrefab, this.PlayerWeaponRoot, weaponType.TpsPrefab.transform.position, weaponType.TpsPrefab.transform.rotation);
            this.SelectWeapon = gameObject.GetComponent<TPWeaponShooter>();
            this.SelectWeapon.name = weaponType.Name;
            this.WeaponsList.Add(this.SelectWeapon);
            this.SelectWeapon.Init(weaponType.ID, skinID, this);
            if (fireStat > 0)
            {
                this.SelectWeapon.SetFireStat(fireStat);
            }
            this.SelectWeapon.SetStickers(stickers);
        }
        else
        {
            this.SelectWeapon = tpweaponShooter;
            this.SelectWeapon.Data.skin = skinID;
            this.SelectWeapon.UpdateSkin();
            if (fireStat > 0)
            {
                this.SelectWeapon.UpdateFireStat(fireStat);
            }
            this.SelectWeapon.SetStickers(stickers);
        }
        this.SelectWeapon.Active();
        this.Sounds.Stop();
    }

    private TPWeaponShooter ContainsWeapon(string weaponName)
    {
        for (int i = 0; i < this.WeaponsList.Count; i++)
        {
            if (weaponName == this.WeaponsList[i].name)
            {
                return this.WeaponsList[i];
            }
        }
        return null;
    }

    public void Fire(DecalInfo decalInfo)
    {
        nProfiler.BeginSample("PlayerSkin.Fire");
        if (this.SelectWeapon != null)
        {
            this.SelectWeapon.Fire(this.visible, decalInfo);
        }
        DecalsManager.FireWeapon(decalInfo);
        nProfiler.EndSample();
    }

    public void Reload()
    {
        if (this.SelectWeapon != null)
        {
            this.PlayerAnimator.reload = true;
            this.SelectWeapon.Reload();
        }
    }

    public void Damage(DamageInfo damageInfo)
    {
        if (this.Dead || (this.PlayerTeam == damageInfo.team && !GameManager.friendDamage))
        {
            return;
        }
        if (this.StartDamage)
        {
            return;
        }
        if (this.ShowDamage)
        {
            UIToast.Show(Localization.Get("Damage", true) + ": " + damageInfo.damage, 2f);
        }
        UICrosshair.Hit();
        this.Controller.Damage(damageInfo);
    }

    public void ActiveRagdoll(Vector3 force, bool head)
    {
        if (CameraManager.type == CameraType.FirstPerson && UISpectator.GetActive() && CameraManager.selectPlayer == this.Controller.photonView.ownerId)
        {
            CameraManager.SetType(CameraType.Spectate, new object[]
            {
                this.Controller.photonView.ownerId
            });
            this.visible = true;
        }
        if (this.visible)
        {
            if (force.magnitude < 0.5f)
            {
                force = new Vector3(0f, 0f, (float)UnityEngine.Random.Range(-1, 1));
            }
            this.Sounds.Stop();
            if (!this.activeRagdoll)
            {
                if (!this.Timer.Contains("DeactiveRagdoll"))
                {
                    this.Timer.Create("DeactiveRagdoll", 2f, new TimerDelegate(this.DeactiveRagdoll));
                }
                this.Timer.In("DeactiveRagdoll", 2f);
            }
            this.activeRagdoll = true;
            if (this.SelectWeapon != null && !DropWeaponManager.enable)
            {
                this.SelectWeapon.SetParent(this.PlayerRagdoll.playerRightWeaponRoot, this.PlayerRagdoll.playerLeftWeaponRoot);
            }
            this.PlayerRagdoll.Active(force, this.PlayerLimbs);
            this.PlayerRoot.SetActive(false);
        }
        else
        {
            if (this.activeRagdoll)
            {
                this.DeactiveRagdoll();
            }
            this.PlayerRoot.SetActive(false);
        }
    }

    public void DeactiveRagdoll()
    {
        if (this.SelectWeapon != null && !DropWeaponManager.enable)
        {
            this.SelectWeapon.SetParent(this.PlayerWeaponRoot, this.PlayerTwoWeaponRoot);
        }
        this.activeRagdoll = false;
        this.PlayerRagdoll.Deactive();
    }

    private void UpdateFoosteps()
    {
        this.Foostep = true;
        this.cachedAudioSource.pitch = UnityEngine.Random.Range(1f, 1.5f);
        this.cachedAudioSource.clip = this.PlayerFoosteps[UnityEngine.Random.Range(0, this.PlayerFoosteps.Length)];
        this.cachedAudioSource.Play();
        if (!this.Timer.Contains("UpdateFoosteps"))
        {
            this.Timer.Create("UpdateFoosteps", 0.3f, delegate ()
            {
                this.Foostep = false;
            });
        }
        this.Timer.In("UpdateFoosteps", 0.3f);
    }

    public void SetPosition(Vector3 pos)
    {
        this.PhotonPosition = pos;
        this.PlayerTransform.localPosition = this.PhotonPosition;
    }

    public void SetRotation(Vector3 rot)
    {
        this.PhotonRotation = Quaternion.Euler(rot);
        this.PlayerTransform.localRotation = this.PhotonRotation;
    }

    public void SetSkin(int head, int body, int legs)
    {
        this.HeadSkin = head;
        this.BodySkin = body;
        this.LegsSkin = legs;
    }

    public void UpdateSkin()
    {
        UIAtlas atlas = (this.PlayerTeam != Team.Blue) ? GameSettings.instance.PlayerAtlasRed : GameSettings.instance.PlayerAtlasBlue;
        if (this.HeadSkin == 99)
        {
            atlas = GameSettings.instance.PlayerAtlasRed;
        }
        this.Timer.In(0.01f, delegate ()
        {
            if (this.PlayerAtlas != null)
            {
                this.PlayerAtlas.atlas = atlas;
                this.PlayerAtlas.SetSprite("0-" + this.HeadSkin, "1-" + this.BodySkin, "2-" + this.LegsSkin);
                return;
            }
        });
        this.Timer.In(UnityEngine.Random.Range(0.1f, 0.12f), delegate ()
        {
            this.PlayerRagdoll.SetSkin(atlas, this.HeadSkin.ToString(), this.BodySkin.ToString(), this.LegsSkin.ToString());
        });
    }

    public void StartDamageTime()
    {
        if (!GameManager.startDamage)
        {
            return;
        }
        this.StartDamage = true;
        if (!this.Timer.Contains("StartDamageTime"))
        {
            this.Timer.Create("StartDamageTime", GameManager.startDamageTime, delegate ()
            {
                this.StartDamage = false;
            });
        }
        this.Timer.In("StartDamageTime", GameManager.startDamageTime);

    }
}
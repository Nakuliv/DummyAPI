﻿using Mirror;
using RemoteAdmin;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Exiled.API.Enums;
using InventorySystem;
using Exiled.API.Features;

namespace DummyAPI
{
    public class Dummy
    {
        private ItemType helditem;
        public GameObject GameObject { get; internal set; }

        public Player Player { get; internal set; }

        /// <summary>
        /// Get / Set the current Role of the Dummy
        /// </summary>

        public static Dummy Get(Player p)
        {
            if (Extensions.Dummies.TryGetValue(p.GameObject, out Dummy npc))
            {
                return npc;
            }
            else
            {
                return null;
            }
        }

        public HashSet<RoleType> VisibleForRoles { get; set; } = new HashSet<RoleType>();
        public HashSet<Player> VisibleForPlayers { get; set; } = new HashSet<Player>();

        public RoleType Role
        {
            get => GameObject.GetComponent<CharacterClassManager>().CurClass;
            set
            {
                Despawn();
                GameObject.GetComponent<CharacterClassManager>().CurClass = value;
                Spawn();
            }
        }

        /// <summary>
        /// Get / Set the current Name of the Dummy
        /// </summary>
        public string Name
        {
            get => GameObject.GetComponent<NicknameSync>().Network_myNickSync;
            set => GameObject.GetComponent<NicknameSync>().Network_myNickSync = value;
        }

        /// <summary>
        /// Get / Set the current Position of the Dummy
        /// </summary>
        public Vector3 Position
        {
            get => Player.Position;
            set => Player.Position = value;
        }

        public Vector2 Rotation
        {
            get => Player.Rotation;
            set
            {
                Player.Rotations = value;
                Player.CameraTransform.rotation = Quaternion.Euler(new Vector3(value.x, value.y, 90f));
            }
        }

        /// <summary>
        /// Get / Set the Scale of the Dummy
        /// </summary>
        public Vector3 Scale
        {
            get => Player.Scale;
            set => Player.Scale = value;
        }

        /// <summary>
        /// Get / Set the current Item the Dummy is holding
        /// </summary>
        public ItemType HeldItem
        {
            get => helditem;
            set
            {
                GameObject.GetComponent<Inventory>().NetworkCurItem = new InventorySystem.Items.ItemIdentifier(value, 0);
                helditem = value;
            }
        }

        /// <summary>
        /// Get / Set the BadgeText of the Dummy
        /// </summary>
        public string BadgeName
        {
            get => GameObject.GetComponent<ServerRoles>().MyText;
            set => GameObject.GetComponent<ServerRoles>().SetText(value);
        }

        /// <summary>
        /// Get / Set the BadgeColor of the Dummy
        /// </summary>
        public string BadgeColor
        {
            get => GameObject.GetComponent<ServerRoles>().MyColor;
            set => GameObject.GetComponent<ServerRoles>().SetColor(value);
        }

        public PlayerMovementState Movement
        {
            get => (PlayerMovementState)Player.AnimationController().Network_curMoveState;
            set => Player.AnimationController().Network_curMoveState = (byte)value;
        }

        public MovementDirection Direction { get; set; }

        public virtual bool DisplayInRA { get; set; } = false;
        public virtual bool AffectEndConditions { get; set; } = false;

        public float SneakSpeed { get; set; } = 1.8f;

        public float WalkSpeed { get; set; }

        public float RunSpeed { get; set; }

        //Thanks to GameHunt.I used some of his code for the Dummy API https://github.com/gamehunt/CustomNPCs
        private IEnumerator<float> Update()
        {
            for (; ; )
            {
                yield return MEC.Timing.WaitForSeconds(0.1f);
                if (GameObject == null) yield break;
                if (Direction == MovementDirection.Stop)
                {
                    Player.AnimationController().Networkspeed = new Vector2(0f, 0f);
                    continue;
                }

                var wall = false;
                var speed = 0f;

                switch (Movement)
                {
                    case PlayerMovementState.Sneaking:
                        speed = SneakSpeed;
                        break;

                    case PlayerMovementState.Sprinting:
                        speed = RunSpeed * Extensions.SprintSpeed;
                        break;

                    case PlayerMovementState.Walking:
                        speed = WalkSpeed * Extensions.WalkSpeed;
                        break;
                }

                switch (Direction)
                {
                    case MovementDirection.Forward:
                        Player.AnimationController().Networkspeed = new Vector2(speed, 0f);
                        var pos = Position + Player.CameraTransform.forward / 10 * speed;

                        if (!Physics.Linecast(Position, pos, Player.PlayerMovementSync().CollidableSurfaces))
                            Player.PlayerMovementSync().OverridePosition(pos, 0f, true);
                        else wall = true;
                        break;

                    case MovementDirection.BackWards:
                        Player.AnimationController().Networkspeed = new Vector2(-speed, 0f);
                        pos = Position - Player.CameraTransform.forward / 10 * speed;

                        if (!Physics.Linecast(Position, pos, Player.PlayerMovementSync().CollidableSurfaces))
                            Player.PlayerMovementSync().OverridePosition(pos, 0f, true);
                        else wall = true;
                        break;

                    case MovementDirection.Right:
                        Player.AnimationController().Networkspeed = new Vector2(0f, speed);
                        pos = Position + Quaternion.AngleAxis(90, Vector3.up) * Player.CameraTransform.forward / 10 * speed;

                        if (!Physics.Linecast(Position, pos, Player.PlayerMovementSync().CollidableSurfaces))
                            Player.PlayerMovementSync().OverridePosition(pos, 0f, true);
                        else wall = true;
                        break;

                    case MovementDirection.Left:
                        Player.AnimationController().Networkspeed = new Vector2(0f, -speed);
                        pos = Position - Quaternion.AngleAxis(90, Vector3.up) * Player.CameraTransform.forward / 10 * speed;

                        if (!Physics.Linecast(Position, pos, Player.PlayerMovementSync().CollidableSurfaces))
                            Player.PlayerMovementSync().OverridePosition(pos, 0f, true);
                        else wall = true;
                        break;
                }

                if (wall)
                {
                    Direction = MovementDirection.Stop;
                    Player.AnimationController().Networkspeed = new Vector2(0f, 0f);
                }
            }
        }

        public Dummy(Vector3 pos, Quaternion rot, RoleType role = RoleType.ClassD, string name = "(null)", string badgetext = "", string badgecolor = "") : this(pos, new Vector2(rot.eulerAngles.x, rot.eulerAngles.y), role, name, badgetext, badgecolor) { }

        /// <summary>
        /// Creates a new Dummy and spawns it
        /// </summary>
        /// <param name="pos">The Position where the Dummy should spawn</param>
        /// <param name="rot">The Rotation of the Dummy</param>
        /// <param name="role">The Role which the Dummy should be</param>
        /// <param name="name">The Name of the Dummy</param>
        /// <param name="badgetext">The displayed BadgeText of the Dummy</param>
        /// <param name="badgecolor">The displayed BadgeColor of the Dummy</param>
        public Dummy(Vector3 pos, Vector2 rot, RoleType role = RoleType.ClassD, string name = "(null)", string badgetext = "", string badgecolor = "")
        {
            GameObject obj =
                Object.Instantiate(
                    NetworkManager.singleton.playerPrefab);

            GameObject = obj;

            PlayerManager.AddPlayer(GameObject, default);

            Player ply_obj = new Player(GameObject);
            Player.Dictionary.Add(GameObject, ply_obj);

            Player.IdsCache.Add(ply_obj.Id, ply_obj);

            Player = Player.Get(GameObject);


            Player.GameObject.transform.localScale = Vector3.one;
            Player.GameObject.transform.position = pos;
            Player.PlayerMovementSync().RealModelPosition = pos;
            Rotation = rot;
            Player.QueryProcessor().NetworkPlayerId = QueryProcessor._idIterator;
            Player.QueryProcessor()._ipAddress = Server.Host.IPAddress;
            Player.ClassManager().CurClass = role;
            Player.MaxHealth = Player.ClassManager().Classes.SafeGet((int)Player.Role).maxHP;
            Player.Health = Player.MaxHealth;
            Player.NicknameSync().Network_myNickSync = name;
            Player.RankName = badgetext;
            Player.RankColor = badgecolor;
            Player.IsGodModeEnabled = true;
            RunSpeed = CharacterClassManager._staticClasses[(int)role].runSpeed;
            WalkSpeed = CharacterClassManager._staticClasses[(int)role].walkSpeed;
            MEC.Timing.RunCoroutine(Update());

            NetworkServer.Spawn(GameObject);
            Extensions.Dummies.Add(GameObject, this);
        }

        public void RotateToPosition(Vector3 pos)
        {
            var rot = Quaternion.LookRotation((pos - GameObject.transform.position).normalized);
            Rotation = new Vector2(rot.eulerAngles.x, rot.eulerAngles.y);
        }

        /// <summary>
        /// Despawns the Dummy
        /// </summary>
        public void Despawn()
        {
            NetworkServer.UnSpawn(GameObject);
            Extensions.Dummies.Remove(GameObject);
        }

        /// <summary>
        /// Spawns the Dummy again after Despawning
        /// </summary>
        public void Spawn()
        {
            NetworkServer.Spawn(GameObject);
            Extensions.Dummies.Add(GameObject, this);
        }

        /// <summary>
        /// Destroys the Object
        /// </summary>
        public void Destroy()
        {
            Object.Destroy(GameObject);
            Extensions.Dummies.Remove(GameObject);
        }

        public static Dummy CreateDummy(Vector3 pos, Quaternion rot, RoleType role = RoleType.ClassD, string name = "(null)", string badgetext = "", string badgecolor = "")
            => new Dummy(pos, rot, role, name, badgetext, badgecolor);
    }
}

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Utilities;
using Reactor.Utilities.Attributes;
using Reactor.Utilities.Extensions;
using TMPro;
using TownOfUs.Assets;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Roles.Crewmate;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace TownOfUs.Modules.Components;

[RegisterInIl2Cpp]
[SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Unity")]
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Unity")]
public sealed class AlchemistBrewMinigame(IntPtr cppPtr) : Minigame(cppPtr)
{
    public Transform RolesHolder;
    public GameObject RolePrefab;
    public TextMeshPro StatusText;

    private readonly Color _bgColor = TownOfUsMiraJKColors.Alchemist.SetAlpha(0.8f);
    private RoleTypes? _selectedRole;
    private List<AlchemyIngredient> availableIngredients = [];
    private Action<AlchemyIngredient, int, PassiveButton> clickHandler;
    public static int CurrentCard { get; set; }
    [HideFromIl2Cpp]
    public List<(int, AlchemyIngredient)> ChosenCards { get; set; } = new();

    private void Awake()
    {
        if (Instance)
        {
            Instance.Close();
        }

        RolesHolder = transform.FindChild("Roles");
        RolePrefab = transform.FindChild("RoleCardHolder").gameObject;
        StatusText = transform.FindChild("Status").gameObject.GetComponent<TextMeshPro>();

        StatusText.font = HudManager.Instance.TaskPanel.taskText.font;
        StatusText.fontMaterial = HudManager.Instance.TaskPanel.taskText.fontMaterial;
        StatusText.text = TouLocale.Get("TouJKRoleAlchemistBrewTitle");
        StatusText.gameObject.SetActive(false);
    }

    public static AlchemistBrewMinigame Create()
    {
        var gameObject = Instantiate(TouAssets.RoleSelectionGame.LoadAsset(), HudManager.Instance.transform);
        gameObject.GetComponent<Minigame>().DestroyImmediate();
        gameObject.SetActive(false);

        return gameObject.AddComponent<AlchemistBrewMinigame>();
    }

    [HideFromIl2Cpp]
    public void Open(List<AlchemyIngredient> ingredients, Action<AlchemyIngredient, int, PassiveButton> onClick)
    {
        availableIngredients = ingredients;
        clickHandler = onClick;

        Coroutines.Start(CoOpen(this));
    }

    private static IEnumerator CoOpen(AlchemistBrewMinigame minigame)
    {
        while (ExileController.Instance)
        {
            yield return new WaitForSeconds(0.65f);
        }

        minigame.gameObject.SetActive(true);
        minigame.Begin();
    }

    public override void Close()
    {
        Coroutines.Stop(CoAnimateCards());
        HudManager.Instance.StartCoroutine(HudManager.Instance.CoFadeFullScreen(_bgColor, Color.clear));
        CurrentCard = -1;
        MinigameStubs.Close(this);
    }

    private void Begin()
    {
        HudManager.Instance.StartCoroutine(HudManager.Instance.CoFadeFullScreen(Color.clear, _bgColor));

        StatusText!.gameObject.SetActive(true);

        var z = 0;

        foreach (var ingredient in availableIngredients)
        {
            var card = CreateCard(TouLocale.Get($"TouJKRoleAlchemistIngredient{ingredient}"), AlchemistRole.GetIngredientIcon(ingredient).LoadAsset(), z, AlchemistRole.GetIngredientColor(ingredient));
            card.OnClick.RemoveAllListeners();
            card.OnClick.AddListener((UnityAction)(() => { clickHandler.Invoke(ingredient, z, card); }));

            z++;
        }

        Coroutines.Start(CoAnimateCards());
        TransType = TransitionType.None;
        Begin(null);
    }

    private PassiveButton CreateCard(string ingredientName, Sprite? sprite, float z, Color color)
    {
        var newRoleObj = Instantiate(RolePrefab, RolesHolder);
        var actualCard = newRoleObj!.transform.GetChild(0);
        var ingredientText = actualCard.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
        var ingredientImage = actualCard.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();
        actualCard.transform.GetChild(2).gameObject.Destroy();
        var passiveButton = actualCard.GetComponent<PassiveButton>();
        var buttonRollover = actualCard.GetComponent<ButtonRolloverHandler>();

        passiveButton.OnMouseOver.AddListener((UnityAction)(() =>
        {
            newRoleObj.transform.localPosition = new Vector3(newRoleObj.transform.localPosition.x,
                newRoleObj.transform.localPosition.y, newRoleObj.transform.localPosition.z - 10);
        }));
        passiveButton.OnMouseOut.AddListener((UnityAction)(() =>
        {
            newRoleObj.transform.localPosition = new Vector3(newRoleObj.transform.localPosition.x,
                newRoleObj.transform.localPosition.y, newRoleObj.transform.localPosition.z + 10);
        }));

        var randZ = -10f + z * 5f + Random.RandomRange(-1.5f, 1.5f);
        newRoleObj.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -randZ));
        newRoleObj.transform.localPosition =
            new Vector3(newRoleObj.transform.localPosition.x, newRoleObj.transform.localPosition.y, z);

        ingredientText.text = ingredientName;

        if (sprite != null)
        {
            ingredientImage.sprite = sprite;
        }

        ingredientImage.SetSizeLimit(2.8f);

        buttonRollover.OverColor = color;
        buttonRollover.OutColor = TownOfUsMiraJKColors.Alchemist;
        actualCard.gameObject.GetComponent<SpriteRenderer>().color = TownOfUsMiraJKColors.Alchemist;
        ingredientText.color = color;

        return passiveButton;
    }

    [HideFromIl2Cpp]
    private IEnumerator CoAnimateCards()
    {
        foreach (var o in RolesHolder!.transform)
        {
            var card = o.Cast<Transform>();
            if (card == null)
            {
                continue;
            }

            var child = card.GetChild(0);
            yield return CoAnimateCardIn(child);
            Coroutines.Start(MiscUtils.BetterBloop(child, finalSize: 0.55f, duration: 0.22f, intensity: 0.16f));
            yield return new WaitForSeconds(0.1f);
        }

        CurrentCard = -1;
    }

    private static IEnumerator CoAnimateCardIn(Transform card)
    {
        var randY = (CurrentCard * CurrentCard * 0.5f - CurrentCard) * 0.1f + Random.RandomRange(-0.15f, 0f);
        var randZ = -10f + CurrentCard * 5f + Random.RandomRange(-1.5f, 0f);
        if (CurrentCard == 0)
        {
            randY = 0f;
            randZ = -2f;
        }

        card.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -randZ));
        card.transform.localPosition = new Vector3(card.transform.localPosition.x, card.transform.localPosition.y - 5f,
            card.transform.localPosition.z);
        card.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 14f));
        card.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        card.parent.gameObject.SetActive(true);
        for (var timer = 0f; timer < 0.4f; timer += Time.deltaTime)
        {
            var num = timer / 0.4f;
            card.localPosition =
                new Vector3(card.localPosition.x, Mathf.SmoothStep(-5f, randY, num), card.localPosition.z);
            card.transform.localRotation =
                Quaternion.Euler(new Vector3(0, 0, Mathf.SmoothStep(-randZ + 2.5f, -randZ, num)));
            yield return null;
        }

        CurrentCard++;

        card.localPosition = new Vector3(card.localPosition.x, randY, card.localPosition.z);
        card.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -randZ));
    }
}
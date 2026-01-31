# VR Comfort Settings

Player comfort options for accessible VR gameplay.

---

## Overview

VR comfort varies significantly between players. Providing options allows more players to enjoy the game without motion sickness or discomfort.

---

## Locomotion Options

| Type | Description | Comfort Level |
|:-----|:------------|:--------------|
| Teleport | Point and blink | High |
| Dash | Quick move with brief fade | Medium |
| Smooth | Continuous movement | Low |

### Teleport Implementation

```csharp
public class TeleportLocomotion : MonoBehaviour
{
    [SerializeField] private XRRayInteractor rayInteractor;
    [SerializeField] private TeleportationProvider teleportProvider;
    
    public void OnTeleportActivate(InputAction.CallbackContext context)
    {
        if (rayInteractor.TryGetCurrent3DRaycastHit(out var hit))
        {
            var request = new TeleportRequest
            {
                destinationPosition = hit.point,
                destinationRotation = transform.rotation
            };
            teleportProvider.QueueTeleportRequest(request);
        }
    }
}
```

### Smooth Locomotion with Vignette

```csharp
public class SmoothLocomotion : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private TunnelingVignetteController vignette;
    
    private void Update()
    {
        var input = moveAction.ReadValue<Vector2>();
        
        if (input.magnitude > 0.1f)
        {
            vignette.SetActive(true);
            Move(input);
        }
        else
        {
            vignette.SetActive(false);
        }
    }
}
```

---

## Turning Options

### Snap Turn

Instant rotation by fixed degrees.

| Preset | Degrees |
|:-------|:--------|
| Wide | 45° |
| Normal | 30° |
| Precise | 15° |

```csharp
public class SnapTurnProvider : MonoBehaviour
{
    [SerializeField] private float turnAngle = 45f;
    [SerializeField] private float debounceTime = 0.5f;
    
    private float _lastTurnTime;
    
    public void Turn(float direction)
    {
        if (Time.time - _lastTurnTime < debounceTime) return;
        
        var rotation = Quaternion.Euler(0, turnAngle * Mathf.Sign(direction), 0);
        xrOrigin.RotateAroundCameraPosition(xrOrigin.transform.up, turnAngle * direction);
        _lastTurnTime = Time.time;
    }
}
```

### Smooth Turn

Continuous rotation with variable speed.

```csharp
public class SmoothTurnProvider : MonoBehaviour
{
    [SerializeField] private float turnSpeed = 60f;
    
    private void Update()
    {
        var input = turnAction.ReadValue<float>();
        if (Mathf.Abs(input) > 0.1f)
        {
            xrOrigin.RotateAroundCameraPosition(xrOrigin.transform.up, input * turnSpeed * Time.deltaTime);
        }
    }
}
```

---

## Comfort Aids

### Tunneling Vignette

Reduces peripheral vision during movement to reduce motion sickness.

```csharp
public class TunnelingVignetteController : MonoBehaviour
{
    [SerializeField] private Material vignetteMaterial;
    [SerializeField] private float transitionSpeed = 5f;
    [SerializeField] private float maxVignetteStrength = 0.5f;
    
    private float _targetStrength;
    private float _currentStrength;
    
    public void SetActive(bool active)
    {
        _targetStrength = active ? maxVignetteStrength : 0f;
    }
    
    private void Update()
    {
        _currentStrength = Mathf.Lerp(_currentStrength, _targetStrength, Time.deltaTime * transitionSpeed);
        vignetteMaterial.SetFloat("_VignetteStrength", _currentStrength);
    }
}
```

### Seated Mode

Adjusts player height for seated play.

```csharp
public class SeatedModeController : MonoBehaviour
{
    [SerializeField] private Transform xrOrigin;
    [SerializeField] private float seatedHeightOffset = 0.5f;
    
    public void EnableSeatedMode()
    {
        var currentY = xrOrigin.position.y;
        xrOrigin.position = new Vector3(xrOrigin.position.x, currentY + seatedHeightOffset, xrOrigin.position.z);
    }
    
    public void RecalibrateHeight()
    {
        // Reset to floor level based on current head position
    }
}
```

---

## Settings Data

```csharp
[System.Serializable]
public class ComfortSettings
{
    public LocomotionType locomotionType = LocomotionType.Teleport;
    public TurnType turnType = TurnType.Snap;
    public float snapTurnAngle = 45f;
    public float smoothTurnSpeed = 60f;
    public bool vignetteEnabled = true;
    public float vignetteStrength = 0.5f;
    public bool seatedMode = false;
    public Handedness dominantHand = Handedness.Right;
}

public enum LocomotionType { Teleport, Dash, Smooth }
public enum TurnType { Snap, Smooth }
public enum Handedness { Left, Right }
```

---

## UI Considerations

VR UI must be readable and comfortable:

| Element | Guideline |
|:--------|:----------|
| Distance | 1-2 meters from player |
| Text size | Minimum 16px body, 24px headers |
| Button size | Minimum 64x64 pixels |
| Contrast | High contrast for readability |
| Motion | Avoid rapid animations |

---

## Best Practices

| Practice | Reason |
|:---------|:-------|
| Default to highest comfort | New players need gentle introduction |
| Remember preferences | Save and restore on next session |
| Allow runtime changes | Let players adjust during play |
| Test on device | Editor previews don't show comfort issues |

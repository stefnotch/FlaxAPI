# Custom Visject Surfaces

- `AssetProxy`: A proxy for assets like Animation Graph and Material, allows you to create your own asset
  - Added in the `OnInit` method of the `ContentDatabaseModule`
  - `AnimationGraphProxy`: Proxy for the `AnimationGraph` which is a BinaryAsset
    - A BinaryAssetProxy
    - Opens a `AnimationGraphWindow`
  - `NumberGraphProxy`: Mine! A proxy for the `NumberGraph`, which is *not* a JsonAsset
    - A JsonAssetProxy



## Visject Window

`VisjectWindowBase`: I refactored the Visject windows to have a base class with some shared functionality

- Preview
- Properties Proxy
  - `var parameters = particleEmitterWin.Surface.Parameters;` ! That's different from the animation graph parameters and material parameters, which are directly attached to the asset.
- Visject Surface



The particle emitter **surface** has a **root node**, while the other ones (**windows**) have a **main node**. Refactor this?

```csharp
public class ParticleEmitterSurface : VisjectSurface
{
    internal Particles.ParticleEmitterNode _rootNode;

    /// <summary>
    /// Gets the root node of the emitter graph.
    /// </summary>
    public Particles.ParticleEmitterNode RootNode => _rootNode;
```



`ContentEditingModule.CloneAssetFile` has a weird parameter order. First comes the destination, then the source?

### Visject Windows

- `AnimationGraphWindow`
- `MaterialWindow`
- `ParticleEmitterWindow`
- `NumberGraphWindow`: Mine! It's a Visject surface window with a preview, settings editor and a *visject surface*!



## Preview

- `NumberGraphPreview`: Mine! Inherits from AssetPreview
  - Has an asset
  - Has a number graph "virtual" instance
    - Which gets re-initialized whenever the asset gets changed



## Visject Surface

- `NumberGraphSurface`: Mine! The Visject Surface



Note: `CTRL` + `Shift` + `Space` shows the function signature thingy again



## Json Visject Surface Saving

- `JsonSurface` in the Editor assembly. It can save and load a surface, which is simply a ContentItem next to the real deal
- It saves stuff to a byte[]


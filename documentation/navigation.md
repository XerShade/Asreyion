# Code-defined navigation

Navigation menus are assembled at request time from `IOnBuildNavigationHook` implementations. Each hook receives a builder for one named menu, so hooks should check `builder.MenuName` before adding items. The builder merges items with the same label at the same level, allowing separate hooks to contribute to a shared parent.

Use `AddOrGet` for a root item and `AddOrGetChild` for each nested level. Since each method returns the item definition, nesting can continue by calling `AddOrGetChild` on the returned item. Set routes, icons, item types, and sibling order with the fluent methods on `NavigationItemDefinition`.

Render any named menu with the view component tag helper:

```razor
<vc:navigation-view menu-name="Primary"></vc:navigation-view>
<vc:navigation-view menu-name="Account"></vc:navigation-view>
```

Navigation no longer reads or writes menu records. The legacy entities and EF mappings remain in the model to preserve the existing database schema; the old navigation CRUD service has been removed. Historical migrations remain unchanged.

The Blog hook reads category records and preserves their `ParentId` hierarchy. Category links use the stored `Slug` values for every level, producing paths such as `/blog/category/gaming/runescape`. The category endpoint also accepts the older leaf-only slug URLs.

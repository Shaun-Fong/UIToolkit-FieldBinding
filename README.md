<p align="center">
  <img src="https://user-images.githubusercontent.com/16713354/217272667-fe624616-0b66-4eee-b6d4-ed63b2225f09.png">
</p>

**New [binding system](https://forum.unity.com/threads/introducing-the-runtime-bindings-api-in-unity-2023-2.1454221/) is out.(2023.2+)**

## Binding
- This little tool can save you a lot of time using the `Q<T>` interface. (I'm so tired of written `Q<T>` over and over again...)
- 这个小工具可以帮你在使用Unity的UIToolkit获取指定组件的时候节省非常多的时间。
- One Click to generate binding code.
- 一键生成需要的绑定脚本。
- Search the field what you need.
- 使用检索来查找字段

<p align="center">
  <img src="https://user-images.githubusercontent.com/16713354/217278859-d88412aa-da4c-4262-842e-1b194a76bd0c.gif">
</p>

## Usage 使用
- Assign Source Asset (给上UIDocument的Source Asset)
- Click `Menu/Generate` (点击`Menu/Generate`)
- Select the location where the script was generated in popup files save panel, in example file is `BindTest`, it will generate `BindTest_BindFields`.(在弹出的保存窗口中选中脚本保存的位置，示例中直接选中`BindTest.cs`脚本即可，将会自动生成名为`BindTest_BindFields`脚本文件)
- Next time you can click `Generate` for quick regen.(当你做了UI修改之后，下次可以直接点击`Generate`按钮快速进行重新生成绑定脚本)
- [Example Script](./Assets/Examples/Scripts/BindTest.cs)
- [Example Binding Script](./Assets/Examples/Scripts/BindTest_BindFields.cs)

![image](https://user-images.githubusercontent.com/16713354/217477617-7ae576ad-1d46-447a-9a77-a6db862d0e72.png)

1. Namespace (命名空间)
2. Change the location where the script was generated. (更改脚本生成位置)
3. Generate Button. (点击即可生成)
4. The location where the script was generated. (脚本生成的位置)
5. Script Content Generate preview. (预览)

### Generate/Save/Save As/Select in Menu 菜单按钮选项

![image](https://user-images.githubusercontent.com/16713354/217478834-f7189c95-ab32-4350-a3d7-a2417346628f.png)

Click `Menu` and you can see many options
- `Generate` is for fast generate. `Save File Panel` will popup when you first time generate. (快速生成的按钮)
- `Save` is for saving selected fileds data to disk. (保存选中字段的数据到硬盘，下次加载会自动选中)
- `Save As` is for regen the data.Popup `Save File Panel` every time. (另存为，重新定位脚本)
- `Select` provides a variety of ways to quickly select fields, the most common and default is `ValidName`. (`Select` 提供多种快速选中字段的方式，最常用和默认用的是`ValidName`)

### Filter 过滤

![image](https://user-images.githubusercontent.com/16713354/217481441-d85c2580-f2c2-4164-aba8-350cc09aa92b.png)

You can open the `Filter` option by clicking the button in the upper right corner.

你可以通过点击右上角的按钮打开`过滤`选项

### Valid Fileds? 有效字段
The `element name` in `UIBuilder` rules are as follows below.Like you normally declare a variable.
在`UIBuilder`中的元素名称的规则跟你正常声明一个变量的规则就行。
- The name can contain letters, digits, and the underscore character (_).
- The first character of the name must be a letter. The underscore is also a legal first character, but its use is not recommended at the beginning of a name. An underscore is often used with special commands, and it's sometimes hard to read.
- Case matters (that is, upper- and lowercase letters). C# is case-sensitive; thus, the names count and Count refer to two different variables.
- C# keywords can't be used as variable names. Recall that a keyword is a word that is part of the C# language. (A complete list of the C# keywords can be found in Appendix B, "C# Keywords.")
![image](https://user-images.githubusercontent.com/16713354/217474726-8eb55b0c-7dfa-43c7-9e15-5edc6d9f8b87.png)

(The field is invalid because the beginning contains invalid characters `-`)
(字段无效是因为开头有一个`-`符号)



## Install

### Via Unity Package Manager 通过PackageManager直接使用Git链接
Add Package From Git URL
`https://github.com/Shaun-Fong/UIToolkit-FieldBinding.git?path=/Packages/com.shaunfong.uitoolkit-binding`

### Or 或者

### Via OpenUPM 通过OpenUPM（推荐）
- open `Edit/Project Settings/Package Manager`(打开PackageManager配置，`Edit/Project Settings/Package Manager`)
- add a new Scoped Registry (or edit the existing OpenUPM entry) (点击右上角齿轮添加一个`Scoped Registry`，添加如下)

Name `package.openupm.com`

URL `https://package.openupm.com`

- click `Save` (or `Apply`) (点击`保存`)
- open `Window/Package Manager` (打开PackageManager)
- click `+` (点击加号)
- select `Add package by name...` or `Add package from git URL...` (点击`Add package by name...` 或 `Add package from git URL...`)
- paste `com.shaunfong.uitoolkit-fieldbinding` into name (粘贴`com.shaunfong.uitoolkit-fieldbinding`)
- paste `1.0.0` into version (粘贴`1.0.0`到版本)
- click `Add` (点击`Add`添加)

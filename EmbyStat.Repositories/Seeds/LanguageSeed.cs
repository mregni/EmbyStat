using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Repositories.Seeds;

public static class LanguageSeed
{
    public static readonly Language[] Languages =
        {
        new() {Id = "082d8aaf-f86a-4401-bf0f-c315b3c9d904", Name = "Nederlands", Code = "nl-NL"},
        new() {Id = "62fe1b5e-3328-450b-b24b-fa16bea58870", Name = "English", Code = "en-US"},
        new() {Id = "c9df5e5c-75f3-46c2-8095-97fde33531d8", Name = "Deutsch", Code = "de-DE"},
        new() {Id = "e1940bfd-00c4-46f9-b9ac-a87a5d92e8ca", Name = "Dansk", Code = "da-DK"},
        new() {Id = "d6401f6b-becf-49e2-b82e-1018b3bf607f", Name = "Ελληνικά", Code = "el-GR"},
        new() {Id = "c0a60a3b-282e-46f5-aa7f-661c88f2edb0", Name = "Español", Code = "es-ES"},
        new() {Id = "91cca672-af55-4d55-899a-798826a43773", Name = "Suomi", Code = "fi-FI"},
        new() {Id = "99142c2f-379e-4a25-879b-ecfe25ee9e7c", Name = "Français", Code = "fr-FR"},
        new() {Id = "a48a2ef9-3b64-4069-8e31-252abb6d07a3", Name = "Magyar", Code = "hu-HU"},
        new() {Id = "282182b9-9332-4266-a093-5ff5b7f927a9", Name = "Italiano", Code = "it-IT"},
        new() {Id = "d8b0ae7b-9ba7-4a51-9d7c-94402b51265d", Name = "Norsk", Code = "no-NO"},
        new() {Id = "f3966f43-3ec6-456e-850f-a2ebfc0b539b", Name = "Polski", Code = "pl-PL"},
        new() {Id = "b21074db-74b9-4e24-8867-34e82c265256", Name = "Brasileiro", Code = "pt-BR"},
        new() {Id = "490c1cb5-b711-4514-aa97-d22ddff2b2fa", Name = "Português", Code = "pt-PT"},
        new() {Id = "6b103b14-20d1-49c0-b7ce-8d701399b64d", Name = "Românesc", Code = "ro-RO"},
        new() {Id = "3e8d27e9-e314-4d57-967f-cf5d84144acf", Name = "Svenska", Code = "sv-SE"},
        new() {Id = "97616a9b-60f9-407a-9a87-b4518da5e5f4", Name = "简体中文", Code = "cs-CZ"}
    };
}
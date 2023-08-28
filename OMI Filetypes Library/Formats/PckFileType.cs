namespace OMI.Formats.Pck
{
    public enum PckFileType : int
    {
        SkinFile = 0,  // *.png
        CapeFile = 1,  // *.png
        TextureFile = 2,  // *.png
        /// <summary>
        /// Unused !
        /// </summary>
        UIDataFile = 3,
        /// <summary>
        /// "0" file
        /// </summary>
        InfoFile = 4,
        /// <summary>
        /// (x16|x32|x64)Info.pck
        /// </summary>
        TexturePackInfoFile = 5,
        /// <summary>
        /// languages.loc/localisation.loc
        /// </summary>
        LocalisationFile = 6,
        /// <summary>
        /// GameRules.grf
        /// </summary>
        GameRulesFile = 7,
        /// <summary>
        /// audio.pck
        /// </summary>
        AudioFile = 8,
        /// <summary>
        /// colours.col
        /// </summary>
        ColourTableFile = 9,
        /// <summary>
        /// GameRules.grh
        /// </summary>
        GameRulesHeader = 10,
        /// <summary>
        /// Skins.pck
        /// </summary>
        SkinDataFile = 11,
        /// <summary>
        /// models.bin
        /// </summary>
		ModelsFile = 12,
        /// <summary>
        /// behaviours.bin
        /// </summary>
        BehavioursFile = 13,
        /// <summary>
        /// entityMaterials.bin
        /// </summary>
        MaterialFile = 14,
    }
}

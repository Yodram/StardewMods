using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Common;
using Pathoschild.Stardew.DataLayers.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Pathoschild.Stardew.DataLayers.Layers.Crops
{
    /// <summary>A data layer which shows whether crops needs to be watered.</summary>
    internal class CropWaterLayer : BaseLayer
    {
        /*********
        ** Fields
        *********/
        /// <summary>The legend entry for dry crops.</summary>
        private readonly LegendEntry Dry;

        /// <summary>The legend entry for watered crops.</summary>
        private readonly LegendEntry Watered;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="translations">Provides translations in stored in the mod folder's i18n folder.</param>
        /// <param name="config">The data layer settings.</param>
        public CropWaterLayer(ITranslationHelper translations, LayerConfig config)
            : base(translations.Get("crop-water.name"), config)
        {
            this.Legend = new[]
            {
               this.Dry = new LegendEntry(translations, "crop-water.watered", Color.Green),
               this.Watered = new LegendEntry(translations, "crop-water.dry", Color.Red)
            };
        }

        /// <summary>Get the updated data layer tiles.</summary>
        /// <param name="location">The current location.</param>
        /// <param name="visibleArea">The tiles currently visible on the screen.</param>
        /// <param name="cursorTile">The tile position under the cursor.</param>
        public override IEnumerable<TileGroup> Update(GameLocation location, Rectangle visibleArea, Vector2 cursorTile)
        {
            Vector2[] visibleTiles = visibleArea.GetTiles().ToArray();

            yield return this.GetGroup(location, visibleTiles, HoeDirt.watered, this.Watered);
            yield return this.GetGroup(location, visibleTiles, HoeDirt.dry, this.Dry);
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get a tile group.</summary>
        /// <param name="location">The current location.</param>
        /// <param name="visibleTiles">The tiles currently visible on the screen.</param>
        /// <param name="state">The watered state to match.</param>
        /// <param name="type">The legend entry for the group.</param>
        private TileGroup GetGroup(GameLocation location, Vector2[] visibleTiles, int state, LegendEntry type)
        {
            TileData[] crops = this.GetCropsByStatus(location, visibleTiles, state).Select(pos => new TileData(pos, type)).ToArray();
            return new TileGroup(crops, outerBorderColor: type.Color);
        }

        /// <summary>Get tiles containing crops not covered by a sprinkler.</summary>
        /// <param name="location">The current location.</param>
        /// <param name="visibleTiles">The tiles currently visible on the screen.</param>
        /// <param name="state">The watered state to match.</param>
        private IEnumerable<Vector2> GetCropsByStatus(GameLocation location, Vector2[] visibleTiles, int state)
        {
            foreach (Vector2 tile in visibleTiles)
            {
                HoeDirt dirt = this.GetDirt(location, tile);
                if (dirt?.crop != null && dirt.state.Value == state)
                    yield return tile;
            }
        }
    }
}

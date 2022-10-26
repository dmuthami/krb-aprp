using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class TerrainTypeListResponse : BaseResponse
    {
        public IEnumerable<TerrainType> TerrainType { get; set; }


        private TerrainTypeListResponse(bool success, string message, IEnumerable<TerrainType> terrainType) : base(success, message)
        {
            TerrainType = terrainType;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public TerrainTypeListResponse(IEnumerable<TerrainType> terrainType) : this(true, string.Empty, terrainType)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public TerrainTypeListResponse(string message) : this(false, message, null)
        { }
    }
}

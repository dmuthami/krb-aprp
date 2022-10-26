﻿using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Services.Communication
{
    public class TerrainTypeResponse : BaseResponse
    {
        public TerrainType TerrainType { get; set; }


        private TerrainTypeResponse(bool success, string message, TerrainType terrainType) : base(success, message)
        {
            TerrainType = terrainType;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="road">Saved road.</param>
        /// <returns>Response.</returns>
        public TerrainTypeResponse(TerrainType terrainType) : this(true, string.Empty, terrainType)
        { }

        /// <summary>
        /// Creates am error response.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Response.</returns>
        public TerrainTypeResponse(string message) : this(false, message, null)
        { }
    }
}

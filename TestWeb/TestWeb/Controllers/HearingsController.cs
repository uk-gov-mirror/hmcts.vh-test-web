﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TestApi.Client;
using TestApi.Contract.Requests;
using TestApi.Contract.Responses;
using TestWeb.Contracts.Responses;
using TestWeb.Mappings;
using VideoApi.Contract.Responses;

namespace TestWeb.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("hearings")]
    [ApiController]
    public class HearingsController : ControllerBase
    {
        private readonly ITestApiClient _testApiClient;
        private readonly ILogger<HearingsController> _logger;

        public HearingsController(ITestApiClient testApiClient, ILogger<HearingsController> logger)
        {
            _testApiClient = testApiClient;
            _logger = logger;
        }

        /// <summary>
        ///     Create a hearing
        /// </summary>
        /// <param name="request">Details on the new hearing</param>
        /// <returns>Full details of created hearing</returns>
        [HttpPost]
        [ProducesResponseType(typeof(HearingDetailsResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateHearingAsync(CreateHearingRequest request)
        {
            _logger.LogDebug("CreateHearingAsync");

            try
            {
                var response = await _testApiClient.CreateHearingAsync(request);
                _logger.LogDebug($"New Hearing Created with id {response.Id}");
                return Created(nameof(CreateHearingAsync), response);
            }
            catch (TestApiException e)
            {
                _logger.LogError(e, "Unable to create hearing");
                return StatusCode(e.StatusCode, e.Response);
            }
        }

        /// <summary>
        ///     Confirm hearing by id
        /// </summary>
        /// <param name="hearingId">Id of the hearing</param>
        /// <param name="request">Update the booking status details</param>
        /// <returns>Confirm a hearing</returns>
        [HttpPatch("{hearingId}", Name = nameof(ConfirmHearingByIdAsync))]
        [ProducesResponseType(typeof(ConferenceDetailsResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ConfirmHearingByIdAsync(Guid hearingId, UpdateBookingStatusRequest request)
        {
            _logger.LogDebug($"ConfirmHearingByIdAsync {hearingId}");

            try
            {
                var response = await _testApiClient.ConfirmHearingByIdAsync(hearingId, request);
                _logger.LogDebug($"Hearing confirmed with id {response.Id}");
                return Created(nameof(ConfirmHearingByIdAsync), response);
            }
            catch (TestApiException e)
            {
                _logger.LogError(e, "Unable to confirm hearing");
                return StatusCode(e.StatusCode, e.Response);
            }
        }

        /// <summary>
        ///    Delete hearings by partial case name or number
        /// </summary>
        /// <param name="request">Partial case name or number text for the hearing</param>
        /// <returns>Number of deleted hearings or conferences</returns>
        [HttpPost("removeTestData")]
        [ProducesResponseType(typeof(DeletedResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteTestDataByPartialCaseTextAsync(DeleteTestHearingDataRequest request)
        {
            _logger.LogDebug($"DeleteHearingsByPartialCaseTextAsync");

            try
            {
                var response = await _testApiClient.DeleteTestDataByPartialCaseTextAsync(request);
                _logger.LogDebug($"{response} hearings deleted");
                return Ok(response);
            }
            catch (TestApiException e)
            {
                _logger.LogError(e, "Unable to delete hearing and/or conference");
                return StatusCode(e.StatusCode, e.Response);
            }
        }

        /// <summary>
        ///    Get all hearings by createdBy
        /// </summary>
        /// <param name="createdBy">The user that created the hearing</param>
        /// <returns>Hearings CreatedBy the user</returns>
        [HttpGet("hearings/{createdBy}", Name = nameof(GetAllHearingsByCreatedByAsync))]
        [ProducesResponseType(typeof(List<HearingResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllHearingsByCreatedByAsync(string createdBy)
        {
            _logger.LogDebug($"GetAllHearingsByCreatedByAsync {createdBy}");

            try
            {
                var allHearingsResponse = await _testApiClient.GetAllHearingsAsync();
                _logger.LogDebug($"Retrieved {allHearingsResponse.Count} hearings in total.");
                var hearings = (from hearing in allHearingsResponse where hearing.CreatedBy.ToLower().Equals(createdBy.ToLower()) select HearingResponseMapper.Map(hearing)).ToList();
                _logger.LogDebug($"Filtered down to {hearings.Count} hearings in total created by '{createdBy}'.");
                return Ok(hearings);
            }
            catch (TestApiException e)
            {
                _logger.LogError(e, $"Unable to fetch hearings");
                return StatusCode(e.StatusCode, e.Response);
            }
        }
    }
}

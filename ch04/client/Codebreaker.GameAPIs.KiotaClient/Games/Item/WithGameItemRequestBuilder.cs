using Codebreaker.Client.Games.Item.Moves;
using Codebreaker.Client.Models;

using Microsoft.Kiota.Abstractions;
namespace Codebreaker.Client.Games.Item;

/// <summary>
/// Builds and executes requests for operations under \games\{gameId}
/// </summary>
public class WithGameItemRequestBuilder : BaseRequestBuilder
{
    /// <summary>The moves property</summary>
    public MovesRequestBuilder Moves
    {
        get =>
        new MovesRequestBuilder(PathParameters, RequestAdapter);
    }
    /// <summary>
    /// Instantiates a new WithGameItemRequestBuilder and sets the default values.
    /// </summary>
    /// <param name="pathParameters">Path parameters for the request</param>
    /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
    public WithGameItemRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/games/{gameId}", pathParameters)
    {
    }
    /// <summary>
    /// Instantiates a new WithGameItemRequestBuilder and sets the default values.
    /// </summary>
    /// <param name="rawUrl">The raw URL to use for the request builder.</param>
    /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
    public WithGameItemRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/games/{gameId}", rawUrl)
    {
    }
    /// <summary>
    /// Deletes a game from the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
    /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
    public async Task DeleteAsync(Action<WithGameItemRequestBuilderDeleteRequestConfiguration>? requestConfiguration = default, CancellationToken cancellationToken = default)
    {
#nullable restore
#else
    public async Task DeleteAsync(Action<WithGameItemRequestBuilderDeleteRequestConfiguration> requestConfiguration = default, CancellationToken cancellationToken = default) {
#endif
        var requestInfo = ToDeleteRequestInformation(requestConfiguration);
        await RequestAdapter.SendNoContentAsync(requestInfo, default, cancellationToken);
    }
    /// <summary>
    /// Gets a game by the given id
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
    /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
    public async Task<Game?> GetAsync(Action<WithGameItemRequestBuilderGetRequestConfiguration>? requestConfiguration = default, CancellationToken cancellationToken = default)
    {
#nullable restore
#else
    public async Task<Game> GetAsync(Action<WithGameItemRequestBuilderGetRequestConfiguration> requestConfiguration = default, CancellationToken cancellationToken = default) {
#endif
        var requestInfo = ToGetRequestInformation(requestConfiguration);
        return await RequestAdapter.SendAsync<Game>(requestInfo, Game.CreateFromDiscriminatorValue, default, cancellationToken);
    }
    /// <summary>
    /// Deletes a game from the database
    /// </summary>
    /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
    public RequestInformation ToDeleteRequestInformation(Action<WithGameItemRequestBuilderDeleteRequestConfiguration>? requestConfiguration = default)
    {
#nullable restore
#else
    public RequestInformation ToDeleteRequestInformation(Action<WithGameItemRequestBuilderDeleteRequestConfiguration> requestConfiguration = default) {
#endif
        var requestInfo = new RequestInformation
        {
            HttpMethod = Method.DELETE,
            UrlTemplate = UrlTemplate,
            PathParameters = PathParameters,
        };
        if (requestConfiguration != null)
        {
            var requestConfig = new WithGameItemRequestBuilderDeleteRequestConfiguration();
            requestConfiguration.Invoke(requestConfig);
            requestInfo.AddRequestOptions(requestConfig.Options);
            requestInfo.AddHeaders(requestConfig.Headers);
        }
        return requestInfo;
    }
    /// <summary>
    /// Gets a game by the given id
    /// </summary>
    /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
    public RequestInformation ToGetRequestInformation(Action<WithGameItemRequestBuilderGetRequestConfiguration>? requestConfiguration = default)
    {
#nullable restore
#else
    public RequestInformation ToGetRequestInformation(Action<WithGameItemRequestBuilderGetRequestConfiguration> requestConfiguration = default) {
#endif
        var requestInfo = new RequestInformation
        {
            HttpMethod = Method.GET,
            UrlTemplate = UrlTemplate,
            PathParameters = PathParameters,
        };
        requestInfo.Headers.Add("Accept", "application/json");
        if (requestConfiguration != null)
        {
            var requestConfig = new WithGameItemRequestBuilderGetRequestConfiguration();
            requestConfiguration.Invoke(requestConfig);
            requestInfo.AddRequestOptions(requestConfig.Options);
            requestInfo.AddHeaders(requestConfig.Headers);
        }
        return requestInfo;
    }
    /// <summary>
    /// Configuration for the request such as headers, query parameters, and middleware options.
    /// </summary>
    public class WithGameItemRequestBuilderDeleteRequestConfiguration
    {
        /// <summary>Request headers</summary>
        public RequestHeaders Headers { get; set; }
        /// <summary>Request options</summary>
        public IList<IRequestOption> Options { get; set; }
        /// <summary>
        /// Instantiates a new WithGameItemRequestBuilderDeleteRequestConfiguration and sets the default values.
        /// </summary>
        public WithGameItemRequestBuilderDeleteRequestConfiguration()
        {
            Options = new List<IRequestOption>();
            Headers = new RequestHeaders();
        }
    }
    /// <summary>
    /// Configuration for the request such as headers, query parameters, and middleware options.
    /// </summary>
    public class WithGameItemRequestBuilderGetRequestConfiguration
    {
        /// <summary>Request headers</summary>
        public RequestHeaders Headers { get; set; }
        /// <summary>Request options</summary>
        public IList<IRequestOption> Options { get; set; }
        /// <summary>
        /// Instantiates a new WithGameItemRequestBuilderGetRequestConfiguration and sets the default values.
        /// </summary>
        public WithGameItemRequestBuilderGetRequestConfiguration()
        {
            Options = new List<IRequestOption>();
            Headers = new RequestHeaders();
        }
    }
}

namespace ssptb.pe.tdlt.storage.common.Validations;
public static class CommonHttpValidation
{
    public static bool ValidHttpResponse(HttpResponseMessage response)
        => response != null && response.IsSuccessStatusCode && response.Content != null;
}

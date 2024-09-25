using FluentValidation;
using ssptb.pe.tdlt.storage.command.Upload;
using System.Text.Json;

namespace ssptb.pe.tdlt.storage.commandvalidator.Upload;
public class UploadJsonCommandValidator : AbstractValidator<UploadJsonCommand>
{
    public UploadJsonCommandValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("FileName is required.")
            .Must(BeAValidFileName).WithMessage("FileName contains invalid characters.");

        RuleFor(x => x.JsonContent)
            .NotEmpty().WithMessage("JsonContent is required.")
            .Must(BeAValidJson).WithMessage("JsonContent is not a valid JSON.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");
    }

    // Validación para el formato del JSON
    private bool BeAValidJson(string jsonContent)
    {
        try
        {
            JsonDocument.Parse(jsonContent);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    // Validación opcional para el nombre del archivo
    private bool BeAValidFileName(string fileName)
    {
        return !fileName.Any(c => Path.GetInvalidFileNameChars().Contains(c));
    }
}

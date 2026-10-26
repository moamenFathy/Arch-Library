using FluentValidation;

namespace Library.Modules.Catalog.Application.Books.Validators;

public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
    public UpdateBookCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Book ID is required.")
            .NotEqual(Guid.Empty).WithMessage("Book ID cannot be empty.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("Author is required.")
            .MaximumLength(150).WithMessage("Author cannot exceed 150 characters.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("CategoryId is required.")
            .NotEqual(Guid.Empty).WithMessage("CategoryId cannot be empty.");
    }
}

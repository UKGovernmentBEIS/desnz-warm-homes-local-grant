# Making frontend changes

We are using the [Government Design System](https://design-system.service.gov.uk/get-started/) (GDS) as a framework for the frontend.

The [GDS library](https://github.com/cabinetoffice/govuk-design-system-dotnet) allows us to implement components and classes from this framework directly to `.cshtml` files found in the `Views` folder.

As this library is not currently published to Nuget we actually have a copy of the library in a nuget package in the /Lib folder of this solution.

To make frontend changes in `.cshtml` files:
- We can directly use components from our copy of the GDS library - the library documentation describes [how to do this](https://github.com/UKGovernmentBEIS/govuk-design-system-dotnet?tab=readme-ov-file#how-to-use).
    - A list of existing components can be found in the GDS documentation [here](https://design-system.service.gov.uk/components/).
    - We can also add new components to this library (see 'Making changes to GovUkDesignSystem library' below).
- We can add basic elements (`div`, `p`, `heading`, `ul` etc.) as long as they also have the relevant GDS classes attached.
    - We can update the default styling of these elements by adding relevant override classes.
    - Both the base classes and the override styling classes can be found in the GDS documentation [here](https://design-system.service.gov.uk/styles/).

Note, we don't need to re-run the service to see these changes reflected in the front-end, they should be visible immediately after refreshing the page.

## Making changes to the GovUkDesignSystem library

If you need to make changes to the GovUkDesignSystem (e.g. to add a new component) then you should:
- Clone the BEIS fork of the repository (currently https://github.com/UKGovernmentBEIS/govuk-design-system-dotnet) and check out the `master` branch.
- Create a branch for you feature
- Develop and commit your changes (don't forget automated tests as applicable)
- Build and package your branch with `dotnet pack -p:PackageVersion=1.0.0-$(git rev-parse --short HEAD) -c Release -o .` in the `GovUkDesignSystem` folder
- Copy the built package to /Lib and delete the old package
- Update the package version in the WH:LG project
- Test that your changes work on the WH:LG site
- Create a PR from your branch back to `master`
- Get the PR reviewed and merged
- From time to time create a PR to merge the `master` branch back to the Cabinet Office repository (https://github.com/cabinetoffice/govuk-design-system-dotnet)

### GOV.UK Frontend

The GovUkDesignSystem project relies on the GOV.UK Frontend NPM package which contains images, fonts, styling, and JavaScript. When updating
the GovUkDesignSystem you may also need to update the GOV.UK Frontend NPM package. To do this:

- Update the version number of the GOV.UK Frontend package in package.json
- Run `npm install`
- Run `npm run update-govuk-assets`
- Run `npm run build`
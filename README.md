# SystemDot

* [What is SystemDot](#what-is-systemdot)
* [How can I contribute?](#how-can-i-contribute)
* [Repository naming conventions](#repository-naming-conventions)
* [Package naming conventions](#package-naming-conventions)
* [Issues](#issues)
* [Standards](#standards)
  * [Architectural Decision Record (ADR)](#architectural-decision-record-adr)
  * [Semantic Versioning](#semantic-versioning)
* [Code of Conduct](#code-of-conduct)
* [License](#license)

This repository is the starting point for the SystemDot organization and projects including documentation, templates and some tooling.

This repository is the starting point for the SystemDot organization and projects including documentation, templates and some tooling.

## What is SystemDot?

SystemDot was formed by a group of enthusiastic .NET engineers as an umbrella for building libraries, utilities and frameworks. SystemDot primarily started as a .NET initiative but as the technology world has become more heterogeneous and engineers polyglot, so has SystemDot. 

SystemDot is place for sharing libraries, utilities and frameworks that accelerate development, educate and make the lives of developers easier.

## How can I contribute?

SystemDot welcomes contributions to both existing and new repos! 

* [Contributing](CONTRIBUTING.md) explains what kinds of contributions we welcome
* [Workflow Instructions](docs/workflow/README.md) explains how to build and test

## Repository naming conventions

| Type                                      | Format                       | Examples              |
|-------------------------------------------|------------------------------|-----------------------|
| Independent Library                       | SystemDot<PackageName>       | SystemDotDomain       |
| Extension to existing .NET package        | SystemDot<DotNetPackage>     | SystemDotHealthChecks |
| Extension to existing third-party package | SystemDot<ThirdPartyPackage> | SystemDotSerilog      |
| Framework                                 | SystemDot.<FrameworkName>    | SystemDotDb           |
|                                           |                              | SystemDotInterstellar |

## Package naming conventions

| Type                                      | Format | Examples |
|-------------------------------------------|--------|----------|
| Independent Library                       |        |          |
| Extension to existing .NET package        |        |          |
| Extension to existing third-party package |        |          |
| Framework                                 |        |          |

## Issues

This repo should contain issues that are tied to the overall SystemDot organization.

For other issues, please create them in the appropriate sibling project or repos.

## Standards

### Architectural Decision Record (ADR)

At an organisational level, we have adopting the [ADR's](https://adr.github.io/) approach to document the key decisions we take.  We encourage projects to adopt this approach where there is a need.

## Semantic Versioning

We ask all projects to adopt [Semantic Versioning](https://semver.org/) as the veriosning strategy.  This is a widely adopted approach to versioning and allows consumers of the libraries to understand the impact of changes.

## Code of Conduct

This project plans to adopt the code of conduct defined by the [Contributor Covenant](https://contributor-covenant.org) to clarify expected behavior in our community.

## License

SystemDot projects are generally licensed under the permissive licenses such as MIT. See the individual project repos for how they are licenced.
# Security Policy

## Reporting a vulnerability

Please report security vulnerabilities privately to **support@skailar.com**. Do not open a
public issue for security reports.

Include, where possible:

- A description of the vulnerability and its impact.
- Steps to reproduce or a proof of concept.
- The SDK version and target framework affected.

We aim to acknowledge reports within three business days and will keep you updated as we
investigate and ship a fix.

## Supported versions

This SDK is in pre-release (`0.x`). Security fixes are applied to the latest released version.

## Handling API keys

- Skailar API keys (`skl_live_…`) grant billable access to your account. Never commit them to
  source control or embed them in client-side applications.
- Prefer supplying keys through the `SKAILAR_API_KEY` environment variable or a secret manager.
- The SDK sends the key only in the `Authorization` header to the configured base URL, and this
  header cannot be overridden by user-supplied default or per-call headers.

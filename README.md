# Social Media

Social Media is a project I started to show and enchance my skills in developing and designing both software and the infrastructure of a distributed system. I decided on using a serverless architecture as it's something I don't have much experience in, plus it's very easy to build and deploy using AWS Lambda.

## Environments

Built to support three environments, the social media system can behave differently for each.

- `PRODUCTION` as the name states, this environment is fairly self-explanitory in that the system should operate is if it's in an production environment.
- `DEVELOPMENT` again, fairy self-explainitory, development is very similar to `PRODUCTION`, however, it's mainly used to target a set of development resources, other than production.
- `TEST` this is used for a testing enviornment and should be used for running UNIT tests. Some functions require extra resources such as, configuration files when running in `TEST`.

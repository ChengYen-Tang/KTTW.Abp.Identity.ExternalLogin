# AD login example
Example of using AD to login and synchronize user information on abp framework

## How to use
1. The account password of ad can be found in [config/users.ldif](./config/users.ldif)

1. Start virtual ad server
    ```
    docker run -d --rm --name virtual-ad -v $pwd/config:/ldap/config -p 389:10389 kenneth850511/ldap-ad-it
    ```

1. Start redis server
    ```
    docker run -d --rm --name some-redis -p 6379:6379 redis
    ```

1. Start backend project
    ```
    dotnet watch run -c Release --project .\src\ActiveDirectory.Ldap.Example.HttpApi.Host\
    ```

1. Start front end project
    ```
    dotnet watch run -c Release --project .\src\ActiveDirectory.Ldap.Example.Blazor\
    ```
1. After login with an AD account, go to the database to check the synchronization of user information.

aiur() { arg="$( cut -d ' ' -f 2- <<< "$@" )" && curl -sL https://github.com/AiursoftWeb/AiurScript/raw/master/$1.sh | sudo bash -s $arg; }

nexus_code="./Nexus"
nexus_path="/opt/apps/Nexus"
dbPassword=$(uuidgen)

archon_code="$nexus_code/src/WebServices/Basic/Archon"
gateway_code="$nexus_code/src/WebServices/Basic/Gateway"
developer_code="$nexus_code/src/WebServices/Basic/Developer"
observer_code="$nexus_code/src/WebServices/Infrastructure/Observer"
probe_code="$nexus_code/src/WebServices/Infrastructure/Probe"
stargate_code="$nexus_code/src/WebServices/Infrastructure/Stargate"
wrapgate_code="$nexus_code/src/WebServices/Infrastructure/Wrapgate"
www_code="$nexus_code/src/WebServices/Business/WWW"
wiki_code="$nexus_code/src/WebServices/Business/Wiki"
status_code="$nexus_code/src/WebServices/Business/Status"
account_code="$nexus_code/src/WebServices/Business/Account"
colossus_code="$nexus_code/src/WebServices/Business/Colossus"
wrap_code="$nexus_code/src/WebServices/Business/Wrap"
ee_code="$nexus_code/src/WebServices/Business/EE"

archon_path="$nexus_path/Archon"
gateway_path="$nexus_path/Gateway"
developer_path="$nexus_path/Developer"
observer_path="$nexus_path/Observer"
probe_path="$nexus_path/Probe"
stargate_path="$nexus_path/Stargate"
wrapgate_path="$nexus_path/Wrapgate"
www_path="$nexus_path/WWW"
wiki_path="$nexus_path/Wiki"
status_path="$nexus_path/Status"
account_path="$nexus_path/Account"
colossus_path="$nexus_path/Colossus"
wrap_path="$nexus_path/Wrap"
ee_path="$nexus_path/EE"

build_to()
{
    code_path="$1"
    dist_path="$2"
    project_name="$3"
    dotnet publish -c Release -o $dist_path $code_path/$project_name.csproj
    cat $dist_path/appsettings.json > $dist_path/appsettings.Production.json
}

install_nexus()
{
    if [[ $(curl -sL ifconfig.me) == "$(dig +short $(uuidgen).$1)" ]]; 
    then
        echo "IP is correct."
    else
        echo "You need to create a wildcard DNS record *.$1 to $(curl -sL ifconfig.me)!"
        return 9
    fi

    curl -sL https://github.com/AiursoftWeb/AiurUI/raw/master/install.sh | bash -s ui.$1

    aiur system/set_aspnet_prod
    aiur install/dotnet
    aiur install/jq
    aiur install/sql_server
    aiur mssql/config_password $dbPassword

    aiur git/clone_to AiursoftWeb/Nexus $nexus_code
    build_to $archon_code $archon_path "Aiursoft.Archon"
    build_to $gateway_code $gateway_path "Aiursoft.Gateway"
    build_to $developer_code $developer_path "Aiursoft.Developer"
    build_to $observer_code $observer_path "Aiursoft.Observer"
    build_to $probe_code $probe_path "Aiursoft.Probe"
    build_to $stargate_code $stargate_path "Aiursoft.Stargate"
    build_to $wrapgate_code $wrapgate_path "Aiursoft.Wrapgate"
    build_to $www_code $www_path "Aiursoft.WWW"
    build_to $wiki_code $wiki_path "Aiursoft.Wiki"
    build_to $status_code $status_path "Aiursoft.Status"
    build_to $account_code $account_path "Aiursoft.Account"
    build_to $colossus_code $colossus_path "Aiursoft.Colossus"
    build_to $wrap_code $wrap_path "Aiursoft.Wrap"
    build_to $ee_code $ee_path "Aiursoft.EE"
    rm $nexus_code -rf

    # aiur text/edit_json "ConnectionStrings.DatabaseConnection" "$connectionString" $kahla_path/appsettings.Production.json
    # aiur text/edit_json "KahlaAppId" "$2" $kahla_path/appsettings.Production.json
    # aiur text/edit_json "KahlaAppSecret" "$3" $kahla_path/appsettings.Production.json
    # aiur services/register_aspnet_service "kahla" $port $kahla_path "Kahla.Server"
    # aiur caddy/add_proxy $1 $port

    # Finish the installation
    echo "Successfully installed Nexus as a service in your machine! Please open https://www.$1 to try it now!"
    echo "The port 1433 is not opened. You can open your database to public via: sudo ufw allow 1433/tcp"
    echo "Database identity: $1:1433 with username: sa and password: $dbPassword"
}

install_nexus "$@"
# Performance testing VM environment #

## Run performance testing ##
1. Make sure you are at your home directory, e.g., /home/aft.petec@tgpaccessdomain.com

2. Clone the repository by `git clone https://gitlab.com/rayprosoft/betting-speed-test.git`

3. `cd bettingspeedtest`

4. Checkout your branch by `git checkout [your branch name]` (optional)

5. Launch the program with `dotnet run -c QA --project TGS.BettingSpeedTest/TGS.BettingSpeedTest.csproj` (you may set your parameters for your own testing by using `-- -p 1`)


## Run/Debug in local Linux container ##
1. Make sure you have installed docker engine running in Linux container mode

2. Right-click Dockerfile in Rider and click **Run TGS.BettingSpeedTest...**.
Note that the default Dockerfile is for QA.
If you want to test with local OP/GP API, change the configuration from QA to LOCAL in Dockerfile.

Also note **Expose daemon on tcp://localhost:2375 without TLS** setting must be enabled in Docker
https://blog.jetbrains.com/dotnet/2018/07/18/debugging-asp-net-core-apps-local-docker-container/

## Review performance testing results ##
To see performance testing results, you can run queries in Scripts folder with your own query criteria from pgAdmin in the database management server.

## Server ##
10.25.32.130 - UGSDEVPERF01

10.25.32.131 - UGSDEVPERF02


## Software Installation ##
**git**

sudo yum install git

**.NET Core SDK**

https://docs.microsoft.com/en-us/dotnet/core/install/linux-package-manager-centos7


## hosts file ##
The following should exist

10.22.4.48	qa-gp-tgp.tgpaccess.com

10.22.4.20	qa.tgpaccess.com

If you would like to see the existing hosts file, try `cat /etc/hosts`

For brand new game provider, needs to add gp domain for the very first time
type `sudo vi /etc/hosts`
type password when asked
press `Insert`
add new gp domain with internal LB
press `Esc` when done editing
type `:`
type `wq` to save and exit

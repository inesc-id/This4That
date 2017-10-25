# This4That
Secure incentive scheme for Internet of Things data sharing


SETUP

**************************************
HOW TO DEPLOY A MULTICHAIN-NODE
**************************************
Pre-Requisites
	- Ubuntu Machine with Brigde Network (to be visible from the host machine)
	- Windows Machine with Visual 2015

******************************
DEPLOY Multichain NODE on LINUX Machine
******************************

Folder structure after completing the deploy process:
This4That
	-multichain-1.0-alpha-29
		-config-server
	-multichain-explorer-master

1. Get Multichain software
Get Multichain Version 1.0 alpha 29
tar -xvzf multichain-1.0-alpha-29.tar.gz

2. Create a blockchain (the chain name, This4ThatChain, can be changed)

./multichain-util create This4ThatChain -datadir=config-server

3. Goto Config-Server\This4ThatChain\params.dat
	Annotate 'default-network-port' and 'default-rpc-port'

4. Launch the Blockchain node

sudo ./multichaind This4ThatChain -datadir=config-server -daemon

NOTE: if you want to shutdown the node, enter the following command:
	sudo ./multichain-cli This4ThatChain -datadir=config-server stop

******************************
DEPLOY BLOCKCHAIN EXPLORER on LINUX Machine
******************************
NOTES: Any trouble during the setup, consult the README file on https://github.com/MultiChain/multichain-explorer

1. Get Multichain Explorer zip
	https://github.com/MultiChain/multichain-explorer

2. Install dependencies
	sudo apt-get install sqlite3 libsqlite3-dev
	sudo apt-get install python-dev
	sudo apt-get install python-pip
	sudo pip install --upgrade pip
	sudo pip install pycrypto

3. Install Multichain Explorer fo the current user:
	cd multichain-explorer
	python setup.py install --user

4. Configure Multichain.conf
	cd config-server\This4ThatChain
	grep rpc params.dat
	echo "rpcport=<port>" >> multichain.conf

5. Edit Multichain Explorer config file
	cd multichain-explorer
	cp chain1.example.conf chain1.conf
	Uncomment host 0.0.0.0
	change dirname to the directory to the blockchain (ex: ~/This4That/multichain-1.0-alpha-29/config-server/This4ThatChain)

6. Load Blockchain into explorer (make sure that Multichain dameon is running)
	cd multichain-explorer
	python -m Mce.abe --config chain1.conf --commit-bytes 10000 --no-serve

7. Launch the web-server
	python -m Mce.abe --config chain1.conf

******************************
THIS4THAT CLIENT - Windows Client
******************************

1. Connect client node to This4ThatNode
	\This4That-serverNode\multichain-core:
	multichaind.exe This4ThatChain@<linuxIpAddress>:<port> -datadir=.\config-client
	NOTE: annotate the multichain address

2. Allow Multichain Node to connect (execute on Linux Machine)
	multichain-cli This4ThatChain -datadir=config-server grant 17mBvGunRhzHkn1w4KtZM1jygPHhGxeks9vLFo connect,send,receive,mine,admin
	NOTE: the address (17...LFo) is presented in the console after the This4ThatNode tries to connect to the blockchain

3. Connect the client node to This4ThatNode
	start multichaind.exe This4ThatChain@<linuxIpAddress>:<port> -datadir=.\config-client
	NOTE: now the node must connect to the Linux node

4. Go to the This4That-serverNode\multichain-core\config-client and edit the chain_parameters.xml:
	Change <port> according to the <default-rpc-port> defined in \config-client\This4ThatChain\params.dat
	Change <username> and <password> according to file \config-client\This4That\multichain.conf
	Change <ChainName> according to the blockchain name


5. Now the it just launch the Start on Visual Studio to start Thi4That Application.

NOTE: When the application start the console application asks to press any key to continue. Just press any key after the This4That Web-server
load the content.

6. Example of API commands, after console complete the loading process. Use POSTMAN application to invoke HTTP requests:

Import POSTMAN collection with a set of pre-defined requests:
Link: https://www.getpostman.com/collections/c8529e2514cf16064c56


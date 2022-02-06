var myaccount;  //User Account
var web3; 

function awarkMetaMaskJs(){
  if(typeof window.ethereum==="undefined"){
     alert("please install MetaMask")
  }
  else if (window.ethereum) {
    web3 = new Web3(new Web3.providers.HttpProvider('https://speedy-nodes-nyc.moralis.io/cc094aa144010d9dcce9e869/polygon/mainnet'));
    window.ethereum.enable().catch(
      function(response){
        alert(response.message);
      }
    ).then(function(account){
      myaccount = account[0]
      console.log("Account Address："+myaccount)
      //Testing Address, will replace later
      //myaccount = "0x6d9b9604af48c0c443c89b1a4b7c4f97f0e40701"
      getNFTs(myaccount)
    })
  }
}

function getNFTs(account){
  var url = "https://deep-index.moralis.io/api/v2/"+account+"/nft?chain=polygon&format=decimal"
  http(url,(res)=>{
    let nfts
    try {
       nfts = JSON.parse(res)
       console.log(nfts)
       //alert("Wallet connected, you have "+nfts.total+" NFTs")
       if(nfts.total>0 && nfts.result!=undefined){
          UsersNftMap = nfts.result
          console.log(UsersNftMap)
          let urls = ""
          for(let i = 0; i < UsersNftMap.length; i++){
            let metadata = UsersNftMap[i].metadata
            let token_uri = UsersNftMap[i].token_uri 
            if(metadata != null){
              let image = JSON.parse(metadata).image;
              if(image != null){
                console.log(image)
                urls += image
                urls += "|"
              }
            }
            else if(metadata==null && token_uri!=null){
               if(token_uri.indexOf("api.opensea.io")>0){
                   http(token_uri+"?format=json",(_res)=>{
                      let image = JSON.parse(_res).image;
                      if(image != null){
                        console.log(image)
                        urls += image
                        urls += "|"
                      }
                   })
               }
            }
            else{
            }
          }
          
          //console.log(urls)
          //send data back to unity
          unityInstance.SendMessage('Netmanager','NftsDataCall',urls.slice(0,urls.length - 1))
       }
    } catch (error) {console.log('error', error)}
  })
}


function http(url,callback){
  var requestOptions = {
    method: 'GET',
    headers:{
      'Accept': 'application/json',
      'X-API-Key':'db32dLUsYQwTfzJNB1pQR6AfU49yXjaJyhnORuWnslMSW61BVvo7JH5YCSC1M5Fj',
    }
  };
  fetch(url,requestOptions)
  .then(response => response.text())
  .then(result => callback(result))
  .catch(error => console.log('error', error));
}
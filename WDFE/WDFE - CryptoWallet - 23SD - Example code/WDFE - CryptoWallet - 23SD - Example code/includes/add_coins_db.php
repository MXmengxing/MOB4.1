<?Php 
	
	include('db.php');

	// print_r($_POST); 

	$coinName = $_POST['coin_name'];
	$coinPrice = $_POST['coin_price'];
	$amountCoins = $_POST['amount_coins'];
	$totalValue = $_POST['total_value'];
	
	$addCoin = "INSERT INTO cryptofolio (id, name, price, amount, totalValue, bought_on) 
			VALUES (null, '$coinName', '$coinPrice', '$amountCoins', '$totalValue', NOW())";

	if( mysqli_query($con, $addCoin) )
	{
		echo "Succesfully added to your cryptofolio";
	}
	else
	{
		echo "Oops, can not add a coin to your cryptofolio:" . $addCoin . "<br />" . mysqli_error($con);
	}
		 

?>
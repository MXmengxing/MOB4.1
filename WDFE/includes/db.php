<?php
define("HOST","127.0.0.1");
define("USERNAME","root");
define("PASSWORD","");
define("DATABASE","cryptomania");
$con = mysqli_connect(HOST,USERNAME,PASSWORD,DATABASE);
if (mysqli_connect_errno()) { http_response_code(500); echo "db_error"; exit; }
mysqli_set_charset($con,"utf8mb4");

<?php
header('Content-Type: application/json; charset=utf-8');
require __DIR__.'/db.php';
$id = (int)($_POST['id'] ?? 0);
if ($id<=0) { http_response_code(400); echo json_encode(['ok'=>false]); exit; }
$stmt = mysqli_prepare($con,"DELETE FROM cryptofolio WHERE id=?");
mysqli_stmt_bind_param($stmt,'i',$id);
$ok = mysqli_stmt_execute($stmt);
echo json_encode(['ok'=>$ok?true:false]);

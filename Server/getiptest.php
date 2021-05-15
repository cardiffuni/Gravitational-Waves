<?php 
    require 'sql-config.php';
    require 'sql-azure.php';
    require 'sql-connect.php';
    echo "<h1>Get IP</h1>";
    echo "<p>The time is " . date("Y-m-d h:i:sa") . "</p>";

        
    #$hash = $_GET['hash']; 

    $gen_code = $_GET['gen_code'];

    #$realHash = md5($_GET['name'] . $_GET['score'] . $secretKey); 
    #if($realHash == $hash) { 
    if(isset($gen_code)){
        $sth = $dbh->prepare("SELECT `host_ip`, `team` FROM `scores` WHERE `gen_code` LIKE :gen_code");
        try {
            echo "<p>trying code: <b>" . $gen_code . "</b></p>";
            $worked = $sth->execute(array(':gen_code' => $gen_code));
            echo "<p> did it work? " . $worked . "</p>";

            $result = $sth->fetchAll();

            #echo $sth->debugDumpParams();

            if(count($result) > 0) {
                foreach($result as $data) {
                    $host_ip= $data['host_ip'];
                    $team = $data['team'];
                    echo "<p>Host IP: <b>" . $host_ip . "</b>, Team: <b>". $team . "</b></p>";
                }
            } else {
                echo "<p>No Results!</p>";
			}
        } catch(Exception $e) {
            echo '<h1>An error has ocurred.</h1><pre>', $e->getMessage() ,'</pre>';
        }
    } else {
        echo "<p>invalid arguments</p>";
	}
    #} 
    
?>
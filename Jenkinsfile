node('sd_cert_tool') {
    def mvnHome
    //skipDefaultCheckout()
    
    git poll: true, url: 'https://github.com/SDXorg/test-models'
    git poll: true, url: 'https://github.com/ToddFincannon/SDEverywhere'
    git poll: true, url: 'https://github.com/JamesPHoughton/pysd'

   //stage('Preparation') { 
    //   checkout scm: [
    //       $class: 'GitSCM', 
    //       userRemoteConfigs: [[url: 'https://github.com/sdCloud-io/ossToolsVerificationKit.git']]
    //    ], 
    //    poll: false
   //}
   stage('Build') {
      // Run the maven build
         sh "chmod +x *.sh"
      
         //sh "./test.sh"
   }
   stage('Results') {
      archive 'build/*.json'
      stash name: "certification-report-stash", includes: 'build/*.json'
   }
}
node('master') {
    stage('Publication:prepare') { 
        checkout scm: [
           $class: 'GitSCM', 
           userRemoteConfigs: [[url: 'https://github.com/sdCloud-io/ossToolsVerificationKit.git']] 
        ], 
        poll: false
    }
    // stage('Publication:publishing') { 
    //     dir("src/reportGenerator/data") {
    //         sh "rm -f testReport.json"
    //         unstash "certification-report-stash"
    //     }
    //     sh "rm -rf /srv/www/nginx/sdcloud.io/reports/certificationReport"
    //     sh "mkdir -p /srv/www/nginx/sdcloud.io/reports/certificationReport"
    //     dir("src/reportGenerator/src/") {
    //         sh "cp -r ./* /srv/www/nginx/sdcloud.io/reports/certificationReport/"
    //     }
    //     dir("src/reportGenerator/") {
    //         sh "cp -r data /srv/www/nginx/sdcloud.io/reports/certificationReport/"
    //     }
    //     dir("/srv/www/nginx/sdcloud.io/reports/certificationReport/") {
    //         sh "mv reportgeneratorview.html index.html"
    //         sh "mv data/build/* data/"
    //     }
        
    // }
}
properties([
    pipelineTriggers([
        [$class: 'SCMTrigger', scmpoll_spec: 'H/3 * * * *'],
    ])
])

node('sd_cert_tool') {
   def mvnHome
  
    stage('Preparation') {       
        checkout scm: [
            $class: 'GitSCM', 
            branches: [[name: 'origin/origin/1-changed-language-of-repository']], 
            userRemoteConfigs: [[url: 'https://github.com/sdCloud-io/ossToolsVerificationKit.git']]
        ], 
        poll: true
    }

    stage('Fetch branches'){
        dir('build'){
            checkout([$class: 'GitSCM', 
                branches: [[name: '*//* master']],
                doGenerateSubmoduleConfigurations: false, 
                extensions: [[$class: 'RelativeTargetDirectory', 
                relativeTargetDir: 'pysd_repo']], 
                submoduleCfg: [], 
                userRemoteConfigs: [[url: 'https://github.com/JamesPHoughton/pysd.git']]])
            /* sh "rm -rf pysd_repo"
            sh "git clone -b v2.2.2 https://github.com/JamesPHoughton/pysd.git pysd_repo" */

             checkout([$class: 'GitSCM', 
                branches: [[name: '*//* develop']],
                doGenerateSubmoduleConfigurations: false, 
                extensions: [[$class: 'RelativeTargetDirectory', 
                relativeTargetDir: 'sdeverywhere']], 
                submoduleCfg: [], 
                userRemoteConfigs: [[url: 'https://github.com/climateinteractive/SDEverywhere.git']]]) 
            /* sh "rm -rf sdeverywhere"
            sh "git clone -b 0.4.1 https://github.com/climateinteractive/SDEverywhere.git sdeverywhere" */

            checkout([$class: 'GitSCM', 
                branches: [[name: '*/master']],
                doGenerateSubmoduleConfigurations: false, 
                extensions: [[$class: 'RelativeTargetDirectory', 
                relativeTargetDir: 'testModels']], 
                submoduleCfg: [], 
                userRemoteConfigs: [[url: 'https://github.com/SDXorg/test-models.git']]])
        }
    }

    stage('Restore packages') {
        sh 'dotnet restore ./ReportEngine/ReportEngine.sln'
    }

    stage('Build'){
        sh "dotnet build ./ReportEngine/ReportEngine.csproj --configuration Release"
    }

    stage('Publish'){
        withCredentials([string(credentialsId: 'oss_certification_access_token_github', variable: 'TOKEN')]) {
            sh "dotnet publish ./ReportEngine/ReportEngine.csproj"
            sh "dotnet run --project ./ReportEngine/ReportEngine.csproj"
        }
    } 
}

node('master') {
    stage('Publication:prepare') { 
        checkout scm: [
            $class: 'GitSCM',
            branches: [[name: 'origin/origin/1-changed-language-of-repository']],
            userRemoteConfigs: [[url: 'https://github.com/sdCloud-io/ossToolsVerificationKit.git']]
        ], 
        poll: false
    }

     stage('Publication:move report') {
        dir("src/src/reports") {
            checkout(scm: [
                $class: 'GitSCM',
                branches: [[name: '*/master']],
                userRemoteConfigs: [[url: 'https://github.com/ifelseelif/ossToolsVerificationReports.git']]
                ], 
                poll: false,
                changelog : false
            )
                
            sh "python linker.py"
        }
    }

    stage('Publication:build project'){
        dir("src"){
            sh "sudo npm install"
            sh "sudo npm run build"
        }
    }

    stage('Publication:prepare nginx'){
        sh "rm -rf /srv/www/nginx/sdcloud.io/reports/certificationReport"
        sh "mkdir -p /srv/www/nginx/sdcloud.io/reports/certificationReport"

        dir("src/build") {
            sh "cp -r ./* /srv/www/nginx/sdcloud.io/reports/certificationReport/"
        }
    }
}


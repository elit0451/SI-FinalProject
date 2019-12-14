FROM maven:3.6.3-jdk-11-openj9 AS build  
COPY src /usr/src/app/src  
COPY pom.xml /usr/src/app 
COPY .m2/settings.xml /usr/src/app/.m2/settings.xml  
RUN mvn -s /usr/src/app/.m2/settings.xml -f /usr/src/app/pom.xml clean package

FROM openjdk:11
COPY --from=build /usr/src/app/target/RMIConnector-1.0-SNAPSHOT-jar-with-dependencies.jar /usr/app/rmi.jar
ENTRYPOINT ["java","-jar","/usr/app/rmi.jar"]